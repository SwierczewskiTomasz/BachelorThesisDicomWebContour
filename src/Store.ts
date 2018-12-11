import { createStore, applyMiddleware, Store } from "redux";
import logger from "redux-logger";
import thunk from "redux-thunk";
import { handleActions } from "redux-actions";

declare var window: { __REDUX_DEVTOOLS_EXTENSION__: any };

const initialState: AppState = {
    seriesIds: []
};

let store: Store<any>;

const reducers = handleActions({
    undefined
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
