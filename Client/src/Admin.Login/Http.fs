module Admin.Login.Http

open Admin.Login.Types
open Elmish
open Shared.ViewModels
open Fable.PowerPack
open Fable.PowerPack.Fetch.Fetch_types
open Fable.Core.JsInterop

let private loginPromise (info: LoginInfo) = 
    promise {
        let body = toJson info
        let requestProps = 
            [ RequestProperties.Method HttpMethod.POST
              Fetch.requestHeaders [
                HttpRequestHeaders.ContentType "application/json" ]
              RequestProperties.Body !^body ]
        let! response = Fetch.tryFetch "/api/login" requestProps
        match response with
        | Ok response -> 
            let! json = response.text()
            return ofJson<LoginResult> json
        | Error ex -> return LoginError
    }

let login (info: LoginInfo) =

    let successHandler = function
        | Success token -> LoginSuccess token
        | UsernameDoesNotExist -> LoginFailed "Username does not exist"
        | PasswordIncorrect -> LoginFailed "The password you entered is incorrect"
        | JsonFormatIncorrect -> LoginFailed "The request data (login info) is not formatted correctly"
        | LoginError -> LoginFailed "Unknown error occured while logging you in"
    
    Cmd.ofPromise loginPromise info
                  successHandler
                  (fun ex -> LoginFailed "Unknown error occured while logging you in")