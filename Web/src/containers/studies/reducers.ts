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
export const updateStudy = createAction("STUDY/UPDATE", (institutionName: string,
    referringPhysicianName: string, studyDate: string, studyDescription: string) =>
    ({ institutionName, referringPhysicianName, studyDate, studyDescription }));

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


export const getStudyData = (): Thunk =>
    async (dispatch, getState) => {
        {
            dispatch(startTask());

            const instances = getState().instancesIds;

            if (instances.length > 0) {

                const response = await getBuilder<any>(orthancURL, "instances/" + instances[0] + "/study");
                console.warn(response);
                const studyInfo = {
                    institutionName: response.MainDicomTags.InstitutionName,
                    referringPhysicianName: response.MainDicomTags.ReferringPhysicianName,
                    studyDate: response.MainDicomTags.StudyDate,
                    studyDescription: response.MainDicomTags.StudyDescription
                };
                console.log(studyInfo);
                if (studyInfo !== undefined) {
                    dispatch(updateStudy(studyInfo.institutionName, studyInfo.referringPhysicianName, studyInfo.studyDate, studyInfo.studyDescription));
                    console.warn("update");
                }
                else {
                    console.log("getPatientData() failed");
                }
            }
            dispatch(endTask());
        }
    };

function updateStudiesReducer(state: AppState, action) {
    switch (action.type) {
        case "STUDIES/UPDATE":
            return Object.assign({}, state, { studies: action.payload.studies });
        case "STUDY/UPDATE":
            return Object.assign({}, state, {
                institutionName: action.payload.institutionName,
                referringPhysicianName: action.payload.referringPhysicianName,
                studyDate: action.payload.studyDate,
                studyDescription: action.payload.studyDescription
            });
        default:
            return state;
    }
}

export const studiesReducers = {
    [updateStudies.toString()](state: AppState, action) { return { ...state, ...updateStudiesReducer(state, action) }; },
    [updateStudy.toString()](state: AppState, action) { return { ...state, ...updateStudiesReducer(state, action) }; }
};
