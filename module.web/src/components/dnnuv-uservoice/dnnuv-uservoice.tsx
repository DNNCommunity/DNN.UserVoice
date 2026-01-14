import { Component, Host, h } from '@stencil/core';

@Component({
  tag: 'dnnuv-uservoice',
  styleUrl: 'dnnuv-uservoice.scss',
  shadow: true,
})
export class DnnuvUservoice {
  render() {
    return (
      <Host>
        <h1>Ideas</h1>
      </Host>
    );
  }
}
