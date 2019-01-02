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
export interface PatientInfo {
    readonly name?: string;
    readonly birthdate?: string;
    readonly sex?: string;
}

export const updatePatients = createAction("PATIENTS/UPDATE", (patients: Patient[]) => ({ patients }));
export const updatePatient = createAction("PATIENT/UPDATE", (name: string, birthdate: string, sex: string) => ({ name, birthdate, sex }));

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

export const getPatientData = (): Thunk =>
    async (dispatch, getState) => {
        {
            dispatch(startTask());

            const instances = getState().instancesIds;

            if (instances.length > 0) {

                const response = await getBuilder<any>(orthancURL, "instances/" + instances[0] + "/patient");
                const patientInfo: PatientInfo = {
                    name: response.MainDicomTags.PatientName,
                    birthdate: response.MainDicomTags.PatientBirthDate,
                    sex: response.MainDicomTags.PatientSex
                };
                console.log(patientInfo);
                if (patientInfo !== undefined) {
                    dispatch(updatePatient(patientInfo.name, patientInfo.birthdate, patientInfo.sex));
                    console.warn("update");
                }
                else {
                    console.log("getPatientData() failed");
                }
            }
            dispatch(endTask());
        }
    };

function updatePatientsReducer(state: AppState, action) {
    switch (action.type) {
        case "PATIENTS/UPDATE":
            return Object.assign({}, state, { patients: action.payload.patients });
        case "PATIENT/UPDATE":
            return Object.assign({}, state, { name: action.payload.name, birthdate: action.payload.birthdate, sex: action.payload.sex });
        default:
            return state;
    }
}

export const patientsReducers = {
    [updatePatients.toString()](state: AppState, action) { return { ...state, ...updatePatientsReducer(state, action) }; },
    [updatePatient.toString()](state: AppState, action) { return { ...state, ...updatePatientsReducer(state, action) }; }
};
