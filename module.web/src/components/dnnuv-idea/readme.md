# dnnuv-idea



<!-- Auto Generated Below -->


## Properties

| Property              | Attribute | Description                    | Type     | Default     |
| --------------------- | --------- | ------------------------------ | -------- | ----------- |
| `ideaId` _(required)_ | `idea-id` | The ID of the idea to display. | `number` | `undefined` |


## Dependencies

### Used by

 - [dnnuv-uservoice](../dnnuv-uservoice)

### Depends on

- dnn-button
- dnn-input
- dnn-textarea

### Graph
```mermaid
graph TD;
  dnnuv-idea --> dnn-button
  dnnuv-idea --> dnn-input
  dnnuv-idea --> dnn-textarea
  dnn-button --> dnn-modal
  dnn-button --> dnn-button
  dnn-input --> dnn-fieldset
  dnn-textarea --> dnn-fieldset
  dnnuv-uservoice --> dnnuv-idea
  style dnnuv-idea fill:#f9f,stroke:#333,stroke-width:4px
```

----------------------------------------------

*Built with [StencilJS](https://stenciljs.com/)*
