import { ReducerMap, createAction } from "redux-actions";
import { Action, combineReducers } from "redux";
import { Dispatch } from "redux";

import { startTask, endTask } from "../../helpers/asyncActions";
import { getBuilder, orthancURL } from "../../helpers/requestHelper";
import { Thunk } from "../../helpers/Thunk";

export const updateStudies = createAction("STUDIES/UPDATE", (studiesIds: string[]) => ({ studiesIds }));

export const fetchStudies = (getOpts: string): Thunk =>
    async (dispatch, getState) => {
        {
            dispatch(startTask());
            let response = await getBuilder<any>(orthancURL, getOpts);
            console.warn(response);
            const studiesIds: string[] = response.map(r => r.ID);
            console.log(studiesIds);
            if (studiesIds !== undefined) {
                dispatch(updateStudies(studiesIds));
                console.warn("update");
            }
            else {
                console.log("fetchStudies() failed");
            }
            dispatch(endTask());
        }
    };

function updateStudiesReducer(state: AppState, action) {
    switch (action.type) {
        case "STUDIES/UPDATE":
            return Object.assign({}, state, { studiesIds: action.payload.studiesIds });
        default:
            return state;
    }
}

export const studiesReducers = {
    [updateStudies.toString()](state: AppState, action) { return { ...state, ...updateStudiesReducer(state, action) }; }
};
