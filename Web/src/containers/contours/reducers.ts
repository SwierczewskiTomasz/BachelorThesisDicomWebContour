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
        points: {
            x: number;
            y: number;
        }[];
        brushColor: string;
    }[];
    width: number;
    height: number;
}

export const updateContours = createAction("CONTOURS/UPDATE", (contours: Contour[]) => ({ contours: contours }));
export const updateContour = createAction("CONTOUR/UPDATE", (contour: Contour) => ({ contour: contour }));

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

export const setCurrentContur = (guid: string): Thunk =>
    async (dispatch, getState) => {
        {
            dispatch(updateContour(getState().contours.find(c => c.guid === guid)));
        }
    };


function updateContoursReducer(state: AppState, action) {
    switch (action.type) {
        case "CONTOURS/UPDATE":
            return Object.assign({}, state, { contours: action.payload.contours });
        default:
            return state;
    }
}
function updateContourReducer(state: AppState, action) {
    switch (action.type) {
        case "CONTOUR/UPDATE":
            return Object.assign({}, state, { contour: action.payload.contour });
        default:
            return state;
    }
}



export const contoursReducers = {
    [updateContours.toString()](state: AppState, action) { return { ...state, ...updateContoursReducer(state, action) }; },
    [updateContour.toString()](state: AppState, action) { return { ...state, ...updateContourReducer(state, action) }; }
};
