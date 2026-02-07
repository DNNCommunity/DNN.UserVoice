import { ApiException, ProblemDetails, IProblemDetails } from "./services";

/** ModelState validation error format */
interface IModelStateError {
  ModelState: Record<string, string[]>;
}

/** Standard error with message */
interface IErrorWithMessage {
  Message: string;
}

/**
 * Type guard to check if an object looks like a ProblemDetails
 */
function isProblemDetails(obj: unknown): obj is IProblemDetails {
  if (typeof obj !== "object" || obj === null) {
    return false;
  }
  const pd = obj as Record<string, unknown>;
  // ProblemDetails should have at least a title or detail or errors
  return (
    typeof pd.title === "string" ||
    typeof pd.detail === "string" ||
    Array.isArray(pd.errors)
  );
}

/**
 * Type guard to check if an error is a network error (e.g., fetch failed)
 */
function isNetworkError(error: unknown): boolean {
  if (error instanceof TypeError) {
    // Fetch API throws TypeError for network failures
    const message = error.message.toLowerCase();
    return (
      message.includes("failed to fetch") ||
      message.includes("network") ||
      message.includes("load failed") ||
      message.includes("networkerror")
    );
  }
  return false;
}

/**
 * Formats a ProblemDetails object into a user-friendly message
 */
function formatProblemDetails(problem: IProblemDetails): string {
  const messages: string[] = [];

  if (problem.title != null) {
    messages.push(problem.title);
  }

  if (problem.detail != null) {
    messages.push(problem.detail);
  }

  if (problem.errors && problem.errors.length > 0) {
    messages.push(...problem.errors);
  }

  return messages.length > 0 ? messages.join("\n") : "An error occurred";
}

/**
 * Displays an alert dialog with appropriate error message based on the error format.
 * Handles various error formats including:
 * - Network errors (fetch failures)
 * - ProblemDetails (RFC 7807)
 * - ApiException (NSwag generated)
 * - ModelState validation errors (ASP.NET format)
 * - Standard Error objects
 * - Unknown error types
 */
export default function alertError(error: unknown): void {
  // Handle null/undefined
  if (error === null || error === undefined) {
    alert("An unknown error occurred");
    return;
  }

  // Handle string errors
  if (typeof error === "string") {
    alert(error);
    return;
  }

  // Handle network errors (e.g., no internet connection)
  if (isNetworkError(error)) {
    alert("A network error occurred. Please check your internet connection and try again.");
    return;
  }

  // Handle ApiException (from NSwag generated services)
  if (ApiException.isApiException(error)) {
    const apiException = error;

    // Check if result contains ProblemDetails
    if (apiException.result && isProblemDetails(apiException.result)) {
      alert(formatProblemDetails(apiException.result));
      return;
    }

    // Try to parse the response as ProblemDetails
    if (apiException.response != null) {
      try {
        const parsed = JSON.parse(apiException.response) as unknown;
        if (isProblemDetails(parsed)) {
          const problemDetails = ProblemDetails.fromJS(parsed);
          alert(formatProblemDetails(problemDetails));
          return;
        }
        // Check for Message property in response
        if (
          typeof parsed === "object" &&
          parsed !== null &&
          "Message" in parsed &&
          typeof (parsed as Record<string, unknown>).Message === "string"
        ) {
          alert((parsed as Record<string, unknown>).Message as string);
          return;
        }
      } catch {
        // If JSON parsing fails, use the message
      }
    }

    // Fall back to the exception message
    if (apiException.message != null) {
      alert(apiException.message);
      return;
    }
  }

  // Handle non-ApiException objects
  if (typeof error !== "object") {
    try {
      alert(JSON.stringify(error));
    } catch {
      alert(String(error as string | number | boolean));
    }
    return;
  }

  // Handle ProblemDetails directly
  if (isProblemDetails(error)) {
    alert(formatProblemDetails(error));
    return;
  }

  const errorObj = error as Record<string, unknown>;

  // Handle ModelState validation errors (ASP.NET format)
  if (
    "ModelState" in errorObj &&
    errorObj.ModelState !== undefined &&
    errorObj.ModelState !== null &&
    typeof errorObj.ModelState === "object"
  ) {
    const modelState = errorObj.ModelState as Record<string, string[]>;
    const messages: string[] = [];

    for (const fieldKey in modelState) {
      if (Object.prototype.hasOwnProperty.call(modelState, fieldKey)) {
        const fieldErrors = modelState[fieldKey];
        if (Array.isArray(fieldErrors)) {
          messages.push(...fieldErrors);
        }
      }
    }

    if (messages.length > 0) {
      alert(messages.join("\n"));
      return;
    }
  }

  // Handle standard errors with Message property (Pascal case - ASP.NET style)
  if ("Message" in errorObj && typeof errorObj.Message === "string") {
    alert(errorObj.Message);
    return;
  }

  // Handle native Error objects or objects with string message property (camelCase)
  if (error instanceof Error) {
    alert(error.message);
    return;
  }

  if ("message" in errorObj && typeof errorObj.message === "string") {
    alert(errorObj.message);
    return;
  }

  // Fallback: stringify the error object
  try {
    alert(JSON.stringify(error, null, 2));
  } catch {
    alert("An error occurred that could not be displayed");
  }
}

/** Export the error types for use in other modules */
export type { IModelStateError, IErrorWithMessage };
