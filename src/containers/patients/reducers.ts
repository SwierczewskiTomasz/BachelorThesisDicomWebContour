import { ReducerMap, createAction } from "redux-actions";
import { Action, combineReducers } from "redux";
import { Dispatch } from "redux";

import { startTask, endTask } from "../../helpers/asyncActions";
import { getBuilder, orthancURL } from "../../helpers/requestHelper";
import { Thunk } from "../../helpers/Thunk";


export const updatePatients = createAction("PATIENTS/UPDATE", (patientsIds: string[]) => ({ patientsIds }));

export const fetchPatients = (): Thunk =>
    async (dispatch, getState) => {
        {
            dispatch(startTask());
            let patientsIds = await getBuilder<string[]>(orthancURL, "patients");
            console.log(patientsIds);
            if (patientsIds !== undefined) {
                dispatch(updatePatients(patientsIds));
                console.warn("update");
            }
            else {
                console.log("fetchPatients() failed");
            }
            dispatch(endTask());
        }
    };

function updatePatientsReducer(state: AppState, action) {
    switch (action.type) {
        case "PATIENTS/UPDATE":
            return Object.assign({}, state, { patientsIds: action.payload.patientsIds });
        default:
            return state;
    }
}

export const patientsReducers = {
    [updatePatients.toString()](state: AppState, action) { return { ...state, ...updatePatientsReducer(state, action) }; }
};
