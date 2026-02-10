# dnnuv-uservoice



<!-- Auto Generated Below -->


## Properties

| Property                | Attribute   | Description                  | Type     | Default     |
| ----------------------- | ----------- | ---------------------------- | -------- | ----------- |
| `moduleId` _(required)_ | `module-id` | The ID of the current module | `number` | `undefined` |
| `userId`                | `user-id`   | The ID of the current user   | `number` | `undefined` |


## Dependencies

### Depends on

- [dnnuv-ideas](../dnnuv-ideas)
- [dnnuv-idea](../dnnuv-idea)

### Graph
```mermaid
graph TD;
  dnnuv-uservoice --> dnnuv-ideas
  dnnuv-uservoice --> dnnuv-idea
  dnnuv-ideas --> dnn-searchbox
  dnnuv-ideas --> dnn-button
  dnnuv-ideas --> dnn-progress-bar
  dnn-button --> dnn-modal
  dnn-button --> dnn-button
  dnnuv-idea --> dnn-button
  dnnuv-idea --> dnn-input
  dnnuv-idea --> dnn-textarea
  dnn-input --> dnn-fieldset
  dnn-textarea --> dnn-fieldset
  style dnnuv-uservoice fill:#f9f,stroke:#333,stroke-width:4px
```

----------------------------------------------

*Built with [StencilJS](https://stenciljs.com/)*
