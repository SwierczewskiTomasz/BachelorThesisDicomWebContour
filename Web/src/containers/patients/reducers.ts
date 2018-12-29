import { ReducerMap, createAction } from "redux-actions";
import { Action, combineReducers } from "redux";
import { Dispatch } from "redux";

import { startTask, endTask } from "../../helpers/asyncActions";
import { getBuilder, orthancURL } from "../../helpers/requestHelper";
import { Thunk } from "../../helpers/Thunk";

export interface Patient {
    readonly id: string;
    readonly name: string;
}

export const updatePatients = createAction("PATIENTS/UPDATE", (patients: Patient[]) => ({ patients }));

export const fetchPatients = (): Thunk =>
    async (dispatch, getState) => {
        {
            dispatch(startTask());
            const response = await getBuilder<any[]>(orthancURL, "patients?expand");
            const patients = response.map(r => { return { id: r.ID, name: r.MainDicomTags.PatientName }; });
            console.log(patients);
            if (patients !== undefined) {
                dispatch(updatePatients(patients));
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
            return Object.assign({}, state, { patients: action.payload.patients });
        default:
            return state;
    }
}

export const patientsReducers = {
    [updatePatients.toString()](state: AppState, action) { return { ...state, ...updatePatientsReducer(state, action) }; }
};
