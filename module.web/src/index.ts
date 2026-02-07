import "@dnncommunity/dnn-elements";
import { createRouter } from 'stencil-router-v2';

export * from './components';
export * from './services/services';

export const Router = createRouter({
    parseURL: url => {
      let result = `${url.hash.slice(1)}`
      if (result === "") {
        result = "/";
      }
      return result;
    },
    serializeURL: path => {
      const result = new URL(`#/${path}`)
      return result;
    },
});