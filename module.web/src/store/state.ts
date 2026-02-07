import { createStore } from "@stencil/store";
import { LocalizationViewModel } from "../services/services";

/** Defines the shape of the global state store. */
interface IStore {
  /** The localization data for the module. */
  localization: LocalizationViewModel | null;
  
  /** The id of the Dnn module. */
  moduleId: number;
  
  /** The id of the current user. */
  userLoggedIn: boolean;
}

/** Initializes the store with an initial (default) state. */
export const store = createStore<IStore>({
  localization: null,
  moduleId: -1,
  userLoggedIn: false,
});

export default store.state;

interface LocalizationStore {
  viewModel: LocalizationViewModel;
}

const localizationStore = createStore<LocalizationStore>({
  viewModel: new LocalizationViewModel(),
});

export const localizationState = localizationStore.state;
export const resx = localizationStore.state.viewModel; 
