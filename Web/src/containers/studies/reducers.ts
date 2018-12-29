import { ReducerMap, createAction } from "redux-actions";
import { Action, combineReducers } from "redux";
import { Dispatch } from "redux";

import { startTask, endTask } from "../../helpers/asyncActions";
import { getBuilder, orthancURL } from "../../helpers/requestHelper";
import { Thunk } from "../../helpers/Thunk";

export interface Study {
    readonly id: string;
    readonly name: string;
}

export const updateStudies = createAction("STUDIES/UPDATE", (studies: Study[]) => ({ studies }));

export const fetchStudies = (getOpts: string): Thunk =>
    async (dispatch, getState) => {
        {
            dispatch(startTask());
            let response = await getBuilder<any>(orthancURL, getOpts);
            console.warn(response);
            const studies: Study[] = response.map(r => {
                return { id: r.ID, name: r.MainDicomTags.StudyDescription };
            });
            console.log(studies);
            if (studies !== undefined) {
                dispatch(updateStudies(studies));
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
            return Object.assign({}, state, { studies: action.payload.studies });
        default:
            return state;
    }
}

export const studiesReducers = {
    [updateStudies.toString()](state: AppState, action) { return { ...state, ...updateStudiesReducer(state, action) }; }
};
