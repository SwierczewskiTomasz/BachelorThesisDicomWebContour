import { createAction } from "redux-actions";

import { startTask, endTask } from "../../helpers/asyncActions";
import { getBuilder, orthancURL } from "../../helpers/requestHelper";
import { Thunk } from "../../helpers/Thunk";
import { getPatientData } from "../patients/reducers";
import { getStudyData } from "../studies/reducers";
import { fetchContours, removeAllSelectedContour } from "../contours/reducers";

export const updateInstances = createAction("INSTANCES/UPDATE", (instancesIds: string[]) => ({ instancesIds }));
export const updateInstanceDetails = createAction("INSTANCE_DETAILS/UPDATE",
    (pixelSpacing: string | undefined, spacingBetweenSlices: string | undefined) => ({ pixelSpacing, spacingBetweenSlices }));
export const updateInstanceTreeDetails = createAction("TREE_DETAILS/UPDATE",
    (seriesName: string | undefined, studyName: string | undefined, patientName: string | undefined) => ({ seriesName, studyName, patientName }));
export const updateCurrentInstance = createAction("CURRENT_INSTANCE/UPDATE", (currentInstanceId: number) => ({ currentInstanceId }));

interface FrameInstance {
    readonly ID: string;
    readonly IndexInSeries: number;
}

export const fetchInstances = (getOpts: string): Thunk =>
    async (dispatch, getState) => {
        {
            dispatch(startTask());
            let response = await getBuilder<FrameInstance[]>(orthancURL, getOpts);
            // console.warn(response.map(f => f.IndexInSeries));
            response = response.sort((f1, f2) => f1.IndexInSeries - f2.IndexInSeries);
            // console.error(response.map(f => f.IndexInSeries));
            const instancesIds: string[] = response.map(r => r.ID);
            // console.log(instancesIds);
            if (instancesIds !== undefined) {
                // console.warn(instancesIds);
                await dispatch(updateInstances(instancesIds));
                // console.warn("update");
            }
            else {
                console.warn("fetchInstances() failed");
            }
            dispatch(getPatientData());
            dispatch(getStudyData());
            dispatch(setCurrentInstanceInd(0));
            dispatch(endTask());
        }
    };

export const getDetails = (id: string | undefined): Thunk =>
    async (dispatch, getState) => {
        {
            dispatch(startTask());

            if (id !== undefined) {
                let response = await getBuilder<any>(orthancURL, "instances/" + id + "/tags");
                if (response !== undefined) {
                    const pixelSpacing: string | undefined = response["0028,0030"] === undefined ? undefined : response["0028,0030"].Value;
                    const spacingBetweenSlices: string | undefined = response["0018,0088"] === undefined ? undefined : response["0018,0088"].Value;
                    dispatch(updateInstanceDetails(pixelSpacing, spacingBetweenSlices));
                }

                let series = await getBuilder<any>(orthancURL, "instances/" + id + "/series");
                let study = await getBuilder<any>(orthancURL, "instances/" + id + "/study");
                let patient = await getBuilder<any>(orthancURL, "instances/" + id + "/patient");
                const seriesName = series.MainDicomTags.SeriesDescription || "No name";
                const studyName = study.MainDicomTags.StudyDescription || "No name";
                const patientName = patient.MainDicomTags.PatientName || "No name";
                dispatch(updateInstanceTreeDetails(seriesName, studyName, patientName));

            }
            dispatch(endTask());
        }
    };

export const setCurrentInstanceInd = (index: number): Thunk =>
    async (dispatch, getState) => {
        {
            dispatch(startTask());
            dispatch(updateCurrentInstance(index));
            if (index >= 0) {
                dispatch(getDetails(getState().instancesIds[index]));

                dispatch(fetchContours("api/semiautomaticcontour/FetchByDicomIdToDTOs/" + getState().instancesIds[index],
                    "api/manualcontour/FetchByDicomIdToDTOs/" + getState().instancesIds[index]));
                dispatch(removeAllSelectedContour());
            }
            dispatch(endTask());
        }
    };

function updateInstancesReducer(state: AppState, action) {
    switch (action.type) {
        case "INSTANCES/UPDATE":
            return Object.assign({}, state, { instancesIds: action.payload.instancesIds });
        default:
            return state;
    }
}

function updateInstanceTreeDetailsReducer(state: AppState, action) {
    switch (action.type) {
        case "TREE_DETAILS/UPDATE":
            return Object.assign({}, state, { seriesName: action.payload.seriesName, studyName: action.payload.studyName, patientName: action.payload.patientName });
        default:
            return state;
    }
}

function updateInstanceDetailsReducer(state: AppState, action) {
    switch (action.type) {
        case "INSTANCE_DETAILS/UPDATE":
            return Object.assign({}, state, { pixelSpacing: action.payload.pixelSpacing, spacingBetweenSlices: action.payload.spacingBetweenSlices });
        default:
            return state;
    }
}

function updateCurentInctanceReducer(state: AppState, action) {
    switch (action.type) {
        case "CURRENT_INSTANCE/UPDATE":
            return Object.assign({}, state, { currentInstanceId: action.payload.currentInstanceId });
        default:
            return state;
    }
}

export const instancesReducers = {
    [updateInstances.toString()](state: AppState, action) { return { ...state, ...updateInstancesReducer(state, action) }; },
    [updateInstanceDetails.toString()](state: AppState, action) { return { ...state, ...updateInstanceDetailsReducer(state, action) }; },
    [updateInstanceTreeDetails.toString()](state: AppState, action) { return { ...state, ...updateInstanceTreeDetailsReducer(state, action) }; },
    [updateCurrentInstance.toString()](state: AppState, action) { return { ...state, ...updateCurentInctanceReducer(state, action) }; }
};
