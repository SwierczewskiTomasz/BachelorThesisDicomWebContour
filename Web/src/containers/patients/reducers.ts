import { createAction } from "redux-actions";

import { startTask, endTask } from "../../helpers/asyncActions";
import { getBuilder, orthancURL, postBuilder, deleteBuilder } from "../../helpers/requestHelper";
import { Thunk } from "../../helpers/Thunk";
import { updateInstances } from "../instances/reducers";

export interface Patient {
    readonly id: string;
    readonly name: string;
}
export interface PatientInfo {
    readonly name?: string;
    readonly birthdate?: string;
    readonly sex?: string;
    readonly patientId?: string;
}

export const updatePatients = createAction("PATIENTS/UPDATE", (patients: Patient[]) => ({ patients }));
export const updatePatient = createAction("PATIENT/UPDATE", (name: string, birthdate: string, sex: string, patientId: string) => ({ name, birthdate, sex, patientId }));

export const fetchPatients = (): Thunk =>
    async (dispatch, getState) => {
        {
            dispatch(startTask());
            const response = await getBuilder<any[]>(orthancURL, "patients?expand");
            const patients = response.map(r => ({ id: r.ID, name: r.MainDicomTags.PatientName }));
            // console.log(patients);
            if (patients !== undefined) {
                dispatch(updatePatients(patients));
                // console.warn("update");
            }
            else {
                console.warn("fetchPatients() failed");
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

                // console.warn(response);
                const patientInfo: PatientInfo = {
                    patientId: response.ID,
                    name: response.MainDicomTags.PatientName,
                    birthdate: response.MainDicomTags.PatientBirthDate,
                    sex: response.MainDicomTags.PatientSex
                };
                // console.log(patientInfo);
                if (patientInfo !== undefined) {
                    dispatch(updatePatient(patientInfo.name, patientInfo.birthdate, patientInfo.sex, patientInfo.patientId));
                    // console.warn("update");
                }
                else {
                    console.warn("getPatientData() failed");
                }
            }
            dispatch(endTask());
        }
    };

export const anonymizePatient = (name: string, birthdate: string, sex: string): Thunk =>
    async (dispatch, getState) => {
        {
            dispatch(startTask());

            const patientId = getState().patientId;
            const response = await postBuilder<any>(orthancURL, "patients/" + patientId + "/modify",
                {
                    "Replace": {
                        "PatientID": patientId,
                        "PatientName": name,
                        "PatientBirthDate": birthdate,
                        "PatientSex": sex
                    },
                    "Force": true
                });
            await deleteBuilder<any>(orthancURL, "patients/" + patientId);
            dispatch(updatePatient(name, birthdate, sex, response.ID));
            dispatch(updateInstances([]));
        }
        dispatch(endTask());
    };

function updatePatientsReducer(state: AppState, action) {
    switch (action.type) {
        case "PATIENTS/UPDATE":
            return Object.assign({}, state, { patients: action.payload.patients });
        case "PATIENT/UPDATE":
            return Object.assign({}, state, {
                name: action.payload.name,
                birthdate: action.payload.birthdate,
                sex: action.payload.sex,
                patientId: action.payload.patientId
            });
        default:
            return state;
    }
}

export const patientsReducers = {
    [updatePatients.toString()](state: AppState, action) { return { ...state, ...updatePatientsReducer(state, action) }; },
    [updatePatient.toString()](state: AppState, action) { return { ...state, ...updatePatientsReducer(state, action) }; }
};
