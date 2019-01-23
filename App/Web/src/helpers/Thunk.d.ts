import { ThunkAction } from "redux-thunk";
import { Dispatch } from "redux";

export type Thunk<TState = AppState> = ThunkAction<void, TState, never>