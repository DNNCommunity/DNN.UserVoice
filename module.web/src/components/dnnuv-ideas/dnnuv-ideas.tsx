import { Component, Fragment, Host, State, h } from '@stencil/core';
import state from '../../store/state';
import { IdeaClient, IdeaViewModel, SearchIdeasDto } from '../../services/services';
import alertError from '../../services/alert-error';
import { Icon } from '../../icons/icons';

@Component({
  tag: 'dnnuv-ideas',
  styleUrl: 'dnnuv-ideas.scss',
  shadow: true,
})
export class DnnuvIdeas {

  @State() ideas: IdeaViewModel[] | undefined;

  @State() query: string = '';
  
  private readonly ideaClient: IdeaClient;

  constructor() {
    this.ideaClient = new IdeaClient({moduleId: state.moduleId});
  }

  async componentWillLoad() {
    const dto = new SearchIdeasDto({
      onlyMyIdeas: false,
      pageSize: 10,
      page: 1,
      query: '',
    });
    try {
      const vm = await this.ideaClient.searchIdeas(dto);
      this.ideas = vm?.items;
    } catch (error) {
      alertError(error);
    }
  }

  render() {
    return (
      <Host>
        <div class="top-controls">
          <dnn-searchbox
            placeholder={state.localization?.uI?.searchIdeasPlaceholder}
            onQueryChanged={e => this.query = e.detail}
          />
          {this.query != "" && state.userLoggedIn &&(
            <dnn-button
              onClick={() => location.replace(`#/idea/-1?title=${encodeURIComponent(this.query)}`)}
            >
              <div class="icon-wrapper">
                <Icon.add />
                {state.localization?.uI?.postIdea}
              </div>
            </dnn-button>
          )}
        </div>
        {!state.userLoggedIn && (
          <div class="info">
            {state.localization?.uI?.loginToPost}
          </div>
        )}
        <div class="ideas">
          {this.ideas?.map(idea => (
            <Fragment>
              <div class="vote">
                <div class="vote-box">
                  <div class="vote-count">
                    {Math.floor(Math.random() * 200)} {/* TODO: replace with real vote count */}
                  </div>
                  <div class="vote-label">
                    {state.localization?.uI?.votes}
                  </div>
                </div>
                <dnn-button>
                  {state.localization?.uI?.vote}
                </dnn-button>
              </div>
              <a
                href={`#/idea/${idea.id}`}
                class="idea-summary"
              >
                <h3>{idea.title}</h3>
                <p>{idea.description}</p>
              </a>
            </Fragment>
          ))}
        </div>
      </Host>
    );
  }
}
