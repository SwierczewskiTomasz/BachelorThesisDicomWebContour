import { ReducerMap, createAction } from "redux-actions";
import { Action, combineReducers } from "redux";
import { Dispatch } from "redux";

import { startTask, endTask } from "../../helpers/asyncActions";
import { getBuilder, orthancURL } from "../../helpers/requestHelper";
import { Thunk } from "../../helpers/Thunk";
import { getPatientData } from "../patients/reducers";
import { getStudyData } from "../studies/reducers";

export interface Contour {
    guid: string;
    dicomid: string;
    tag: string;
    lines: {
        x: number,
        y: number
    }[];
    width: number;
    height: number;
}

export const updateContours = createAction("CONTOURS/UPDATE", (contours: Contour[]) => ({ contours: contours }));

export const fetchContours = (getOpts: string): Thunk =>
    async (dispatch, getState) => {
        {
            dispatch(startTask());
            let response = await getBuilder<Contour[]>(orthancURL, getOpts);
            if (response !== undefined) {
                dispatch(updateContours(response));
                console.warn("update contours");
            }
            else {
                console.log("fetchContours() failed");
            }
            dispatch(endTask());
        }
    };


function updateContoursReducer(state: AppState, action) {
    switch (action.type) {
        case "INSTANCES/UPDATE":
            return Object.assign({}, state, { contours: action.payload.contours });
        default:
            return state;
    }
}



export const contoursReducers = {
    [updateContours.toString()](state: AppState, action) { return { ...state, ...updateContoursReducer(state, action) }; }
};
