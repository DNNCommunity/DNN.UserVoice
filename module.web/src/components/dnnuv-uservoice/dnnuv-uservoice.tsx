import { Component, Host, Prop, h } from '@stencil/core';
import state from '../../store/state';
import { Route, match } from "stencil-router-v2";
import { Router } from '../../index';
import { LocalizationClient } from "../../services/services";
import alertError from '../../services/alert-error';

@Component({
  tag: 'dnnuv-uservoice',
  styleUrl: 'dnnuv-uservoice.scss',
  shadow: true,
})
export class DnnuvUservoice {

  /** The ID of the current module */
  @Prop() moduleId!: number;

  /** The ID of the current user */
  @Prop() userId: number;

  private readonly localizationClient: LocalizationClient;

  constructor() {
    this.localizationClient = new LocalizationClient({moduleId: this.moduleId});
  }

  async componentWillLoad() {
    state.moduleId = this.moduleId;
    state.userLoggedIn = this.userId != null && !isNaN(this.userId) && this.userId > 0;
    try {
      state.localization = await this.localizationClient.getLocalization();
    } catch (error) {
      alertError(error);
    }
  }

  render() {
    return (
      <Host>
        <Router.Switch>
          <Route path="/">
            <dnnuv-ideas />
          </Route>
          <Route path={match("/idea/:ideaId")}
            render={({ideaId}) => {
              const params = new URLSearchParams(location.hash.split('?')[1]);
              const title = params.get('title') ?? '';
              return <dnnuv-idea ideaId={parseInt(ideaId)} title={title} />;
            }}
          />
        </Router.Switch>
      </Host>
    );
  }
}
