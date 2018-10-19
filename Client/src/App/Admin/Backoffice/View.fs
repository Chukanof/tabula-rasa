module Admin.Backoffice.View

open Admin.Backoffice.Types
open Fable.Helpers.React.Props
open Fable.Helpers.React

let leftIcon name = span [ Style [ Margin 10 ] ] [ i [ ClassName(sprintf "fa fa-%s" name) ] [] ]

let cardContainer child =
    div [ ClassName "card admin-section" ] [ div [ ClassName "card-block" ] [ div [ ClassName "card-title"
                                                                                    Style [ Margin 20 ] ] [ child ] ] ]

let stories =
    div [] [ h3 [] [ leftIcon "book"
                     str "Stories" ]
             p [] [ str "Stories that you have published for the world to see." ] ]
    |> cardContainer

let drafts =
    div [] [ h3 [] [ leftIcon "file-text-o"
                     str "Drafts" ]
             p [] [ str "Articles that you are still working on and havn't published yet." ] ]
    |> cardContainer

let settings =
    div [] [ h3 [] [ leftIcon "cogs"
                     str "Settings" ]
             p [] [ str "View and edit the settings of the blog and your profile." ] ]
    |> cardContainer

let writeArticle =
    div [] [ h3 [] [ leftIcon "plus"
                     str "New Article" ]
             p [] [ str "A story is the best way to share your ideas with the world." ] ]
    |> cardContainer

let subscribers =
    div [] [ h3 [] [ leftIcon "users"
                     str "Subscribers" ]
             p [] [ str "View who subscribes to your blog" ] ]
    |> cardContainer

let oneThirdPage child page dispatch =
    div [ ClassName "col-md-4"
          OnClick(fun _ -> dispatch (NavigateTo page)) ] [ child ]

let logout dispatch =
    div [ ClassName "col-md-4"
          OnClick(fun _ -> dispatch Logout) ] [ cardContainer <| div [] [ h3 [] [ leftIcon "power-off"
                                                                                  str "Logout" ]
                                                                          p [] [ str "Return to your home page" ] ] ]

let homePage dispatch =
    div [ Style [ PaddingLeft 30 ] ] [ div [ ClassName "row" ] [ oneThirdPage stories PublishedPosts dispatch
                                                                 oneThirdPage drafts Drafts dispatch
                                                                 oneThirdPage settings Settings dispatch
                                                                 oneThirdPage writeArticle NewPost dispatch
                                                                 logout dispatch ] ]

let render currentPage (state : State) dispatch =
    match currentPage with
    | Home -> homePage dispatch
    | NewPost -> NewArticle.View.render state.NewArticleState (NewArticleMsg >> dispatch)
    | Drafts -> Drafts.View.render state.DraftsState (DraftsMsg >> dispatch)
    | PublishedPosts -> PublishedPosts.View.render state.PublishedPostsState (PublishedPostsMsg >> dispatch)
    | EditArticle articleId -> EditArticle.View.render state.EditArticleState (EditArticleMsg >> dispatch)
    | Settings -> Settings.View.render state.SettingsState (SettingsMsg >> dispatch)
