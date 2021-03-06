import { createStore, applyMiddleware, Store } from "redux";
import logger from "redux-logger";
import thunk from "redux-thunk";
import { handleActions } from "redux-actions";
import { seriesReducers } from "./containers/series/reducers";
import { asyncActionsReducers } from "./helpers/asyncActions";
import { patientsReducers } from "./containers/patients/reducers";
import { studiesReducers } from "./containers/studies/reducers";
import { instancesReducers } from "./containers/instances/reducers";
import { contoursReducers } from "./containers/contours/reducers";

declare var window: { __REDUX_DEVTOOLS_EXTENSION__: any };

const initialState: AppState = {
    tasksCount: 0,
    series: [],
    patients: [],
    studies: [],
    instancesIds: [],
    name: undefined,
    birthdate: undefined,
    sex: undefined,
    patientId: undefined,
    currentInstanceId: 0,
    contours: [],
    preview: undefined,
    selectedContourGuids: [],
    seriesName: undefined,
    studyName: undefined,
    patientName: undefined
};

let store: Store<any>;

// console.log(asyncActionsReducers);
// console.log(seriesReducers);

const reducers = handleActions({
    ...asyncActionsReducers,
    ...seriesReducers,
    ...patientsReducers,
    ...studiesReducers,
    ...instancesReducers,
    ...contoursReducers
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
