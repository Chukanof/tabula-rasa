module App.State

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open App.Types

let toHash page =
  match page with
  | Posts -> "#latest-posts"
  | Featured -> "#featured"
  | Archive -> "#archive"
  | Contact -> "#contact"
  | Admin -> "#admin"
  | Home -> "#home"

let pageParser: Parser<Page->Page,Page> =
  oneOf [
    map Posts (s "latest-posts")
    map Featured (s "featured")
    map Archive (s "archive")
    map Contact (s "contact")
    map Home (s "home")
    map Admin (s "admin")
  ]

let urlUpdate (result: Option<Page>) model =
  match result with
  | None ->
      model, Cmd.none
  | Some page ->
      { model with CurrentPage = page }, Cmd.ofMsg (ViewPage page)

let init result =
  let initialPage = Home
  let (posts, postsCmd) = Posts.State.init()
  let admin, adminCmd = Admin.State.init()
  let (model, cmd) =
    urlUpdate result
      { CurrentPage = initialPage
        AdminSecurityToken = None
        Admin = admin
        Posts = posts }
  model, Cmd.batch [ cmd
                     Cmd.map PostsMsg postsCmd
                     Cmd.map AdminMsg adminCmd ]

let update msg state =
  match msg with
  | PostsMsg msg ->
      let postsState, postsCmd = Posts.State.update state.Posts msg 
      let appState = { state with Posts = postsState }
      let appCmd = Cmd.map PostsMsg postsCmd
      appState, appCmd
  | AdminMsg msg ->
      let adminState, adminCmd = Admin.State.update msg state.Admin
      { state with Admin = adminState }, Cmd.map AdminMsg adminCmd
  | ViewPage page ->
      let nextState = { state with CurrentPage = page }
      let modifyUrlCmd = Navigation.newUrl (toHash page)
      let reloadCmd =
        match page with 
        | Posts -> Cmd.ofMsg (PostsMsg Posts.Types.Msg.LoadLatestPosts)
        | _ -> Cmd.none
      nextState, Cmd.batch [ modifyUrlCmd; reloadCmd ]
