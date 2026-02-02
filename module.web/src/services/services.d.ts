export declare class ClientBase {
    private sf;
    private moduleId;
    constructor(configuration: ConfigureRequest);
    protected getBaseUrl(_defaultUrl: string, baseUrl?: string): string;
    protected transformOptions(options: RequestInit): Promise<RequestInit>;
}
export declare class LocalizationClient extends ClientBase {
    private http;
    private baseUrl;
    protected jsonParseReviver: ((key: string, value: any) => any) | undefined;
    constructor(configuration: ConfigureRequest, baseUrl?: string, http?: {
        fetch(url: RequestInfo, init?: RequestInit): Promise<Response>;
    });
    /**
     * Gets localization keys and values.
     * @return OK
     */
    getLocalization(signal?: AbortSignal): Promise<LocalizationViewModel | null>;
    protected processGetLocalization(response: Response): Promise<LocalizationViewModel | null>;
}
/** A viewmodel that exposes all resource keys in strong types. */
export declare class LocalizationViewModel implements ILocalizationViewModel {
    /** Localized strings present in the ModelValidation resources. */
    modelValidation?: ModelValidationInfo | undefined;
    /** Localized strings present in the UI resources. */
    uI?: UIInfo | undefined;
    constructor(data?: ILocalizationViewModel);
    init(_data?: any): void;
    static fromJS(data: any): LocalizationViewModel;
    toJSON(data?: any): any;
}
/** A viewmodel that exposes all resource keys in strong types. */
export interface ILocalizationViewModel {
    /** Localized strings present in the ModelValidation resources. */
    modelValidation?: ModelValidationInfo | undefined;
    /** Localized strings present in the UI resources. */
    uI?: UIInfo | undefined;
}
/** Localized strings for the ModelValidation resources. */
export declare class ModelValidationInfo implements IModelValidationInfo {
    /** Gets or sets the DescriptionRequired localized text. */
    descriptionRequired?: string | undefined;
    /** Gets or sets the DescriptionTooLong localized text. */
    descriptionTooLong?: string | undefined;
    /** Gets or sets the IdGreaterThanZero localized text. */
    idGreaterThanZero?: string | undefined;
    /** Gets or sets the ModuleRequired localized text. */
    moduleRequired?: string | undefined;
    /** Gets or sets the TitleRequired localized text. */
    titleRequired?: string | undefined;
    /** Gets or sets the TitleTooLong localized text. */
    titleTooLong?: string | undefined;
    /** Gets or sets the TitleUnique localized text. */
    titleUnique?: string | undefined;
    /** Gets or sets the UserRequired localized text. */
    userRequired?: string | undefined;
    constructor(data?: IModelValidationInfo);
    init(_data?: any): void;
    static fromJS(data: any): ModelValidationInfo;
    toJSON(data?: any): any;
}
/** Localized strings for the ModelValidation resources. */
export interface IModelValidationInfo {
    /** Gets or sets the DescriptionRequired localized text. */
    descriptionRequired?: string | undefined;
    /** Gets or sets the DescriptionTooLong localized text. */
    descriptionTooLong?: string | undefined;
    /** Gets or sets the IdGreaterThanZero localized text. */
    idGreaterThanZero?: string | undefined;
    /** Gets or sets the ModuleRequired localized text. */
    moduleRequired?: string | undefined;
    /** Gets or sets the TitleRequired localized text. */
    titleRequired?: string | undefined;
    /** Gets or sets the TitleTooLong localized text. */
    titleTooLong?: string | undefined;
    /** Gets or sets the TitleUnique localized text. */
    titleUnique?: string | undefined;
    /** Gets or sets the UserRequired localized text. */
    userRequired?: string | undefined;
}
/** Localized strings for the UI resources. */
export declare class UIInfo implements IUIInfo {
    /** Gets or sets the Cancel localized text. */
    cancel?: string | undefined;
    /** Gets or sets the Create localized text. */
    create?: string | undefined;
    /** Gets or sets the Delete localized text. */
    delete?: string | undefined;
    /** Gets or sets the No localized text. */
    no?: string | undefined;
    /** Gets or sets the Save localized text. */
    save?: string | undefined;
    /** Gets or sets the SearchPlaceholder localized text. */
    searchPlaceholder?: string | undefined;
    /** Gets or sets the ShownItems localized text. */
    shownItems?: string | undefined;
    /** Gets or sets the Yes localized text. */
    yes?: string | undefined;
    constructor(data?: IUIInfo);
    init(_data?: any): void;
    static fromJS(data: any): UIInfo;
    toJSON(data?: any): any;
}
/** Localized strings for the UI resources. */
export interface IUIInfo {
    /** Gets or sets the Cancel localized text. */
    cancel?: string | undefined;
    /** Gets or sets the Create localized text. */
    create?: string | undefined;
    /** Gets or sets the Delete localized text. */
    delete?: string | undefined;
    /** Gets or sets the No localized text. */
    no?: string | undefined;
    /** Gets or sets the Save localized text. */
    save?: string | undefined;
    /** Gets or sets the SearchPlaceholder localized text. */
    searchPlaceholder?: string | undefined;
    /** Gets or sets the ShownItems localized text. */
    shownItems?: string | undefined;
    /** Gets or sets the Yes localized text. */
    yes?: string | undefined;
}
export declare class ApiException extends Error {
    message: string;
    status: number;
    response: string;
    headers: {
        [key: string]: any;
    };
    result: any;
    constructor(message: string, status: number, response: string, headers: {
        [key: string]: any;
    }, result: any);
    protected isApiException: boolean;
    static isApiException(obj: any): obj is ApiException;
}
export interface ConfigureRequest {
    moduleId: number;
}
