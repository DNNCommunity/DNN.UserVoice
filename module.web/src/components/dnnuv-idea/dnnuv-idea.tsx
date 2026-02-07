import { Component, Fragment, Host, Prop, State, h } from '@stencil/core';
import { Icon } from '../../icons/icons';
import state from '../../store/state';
import { IdeaClient, IdeaDetailsViewModel, IIdeaDetailsViewModel, SaveIdeaDto } from '../../services/services';
import alertError from '../../services/alert-error';

@Component({
  tag: 'dnnuv-idea',
  styleUrl: 'dnnuv-idea.scss',
  shadow: true,
})
export class DnnuvIdea {
  /** The ID of the idea to display. */
  @Prop() ideaId!: number;

  @State() idea: IIdeaDetailsViewModel = {
    id: -1,
    title: '',
    description: '',
    canEdit: true,
  };

  private readonly ideaClient: IdeaClient
  
  private originalIdea: IdeaDetailsViewModel | null;

  constructor() {
    this.ideaClient = new IdeaClient({ moduleId: state.moduleId });
  }

  async componentWillLoad() {
    if (this.ideaId == -1){
      // check if title is passed as query param
      const params = new URLSearchParams(location.hash.split('?')[1]);
      const title = params.get('title') ?? '';
      if (title.length > 0) {
        this.idea = { ...this.idea, title: title };
      }
      return;
    }

    try {
      this.originalIdea = await this.ideaClient.getIdeaDetails(this.ideaId);
      this.idea = { ...this.originalIdea };
    } catch (error) {
      alertError(error);
    }
  }

  private canSave() {
    if (!this.idea.canEdit) {
      return false;
    }

    return this.originalIdea == null ||
      JSON.stringify(this.idea) !== JSON.stringify(this.originalIdea);
  }

  private async submit(e: Event) {
    e.preventDefault();
    const dto = new SaveIdeaDto({
      id: this.idea.id,
      title: this.idea.title,
      description: this.idea.description,
    });
    try {
      await this.ideaClient.saveIdea(dto);
      location.replace("#/");
    } catch (error) {
      alertError(error);
    }
  }

  render() {
    return (
      <Host>
        <div class="top-controls">
          <dnn-button
            onClick={() => location.replace("#/")}
          >
            <div class="icon-wrapper">
              <Icon.back />
              {state.localization?.uI?.backToIdeas}
            </div>
          </dnn-button>
        </div>
        <form
          onSubmit={e => void this.submit(e)}
        >
          {this.idea.canEdit
          ? 
            <Fragment>
              <dnn-input
                label={state.localization?.uI?.title}
                value={this.idea.title}
                onValueInput={e => this.idea = {...this.idea, title: e.detail as string}}
                readonly={!this.idea.canEdit}
                required
              />
              <dnn-textarea
                label={state.localization?.uI?.description}
                value={this.idea.description}
                onValueInput={e => this.idea = {...this.idea, description: e.detail}}
                readonly={!this.idea.canEdit}
                required
              />
            </Fragment>
          :
            <Fragment>
              <h2>{this.idea.title}</h2>
              <p>{this.idea.description}</p>
            </Fragment>
          }
          <div class="created-info">
            {state.localization?.uI?.createdBy} <strong>{this.idea.createdByUserDisplayName}</strong> {this.idea.createdSince}
          </div>
          <div class="form-actions">
            <dnn-button
              onClick={() => location.replace("#/")}
            >
              <div class="icon-wrapper">
                <Icon.back />
                {state.localization?.uI?.backToIdeas}
              </div>
            </dnn-button>
            {this.idea.canEdit && (
              <dnn-button
                class="submit"
                type="submit"
                disabled={!this.canSave()}
              >
                {this.idea.id === -1 ? state.localization?.uI?.postIdea : state.localization?.uI?.save}
              </dnn-button>
            )}
          </div>
        </form>
      </Host>
    );
  }
}
