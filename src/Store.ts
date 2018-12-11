import { createStore, applyMiddleware, Store, Action } from "redux";
import logger from "redux-logger";
import thunk from "redux-thunk";
import { handleActions } from "redux-actions";
import { seriesReducers } from "./containers/series/reducers";
import { asyncActionsReducers } from "./helpers/asyncActions";

declare var window: { __REDUX_DEVTOOLS_EXTENSION__: any };

const initialState: AppState = {
    tasksCount: 0,
    seriesIds: []
};

let store: Store<any>;

console.log(asyncActionsReducers);
console.log(seriesReducers);

const reducers = handleActions({
    ...asyncActionsReducers,
    ...seriesReducers
}, initialState);

if (process.env["NODE_ENV"] === "production") {
    store = createStore(
        reducers,
        applyMiddleware(
            thunk
        ),
    );
}
else {
    store = createStore(
        reducers,
        window.__REDUX_DEVTOOLS_EXTENSION__ && window.__REDUX_DEVTOOLS_EXTENSION__(),
        applyMiddleware(
            thunk,
            logger
        )
    );
}

export default store;
