import { ReducerMap, createAction } from "redux-actions";
import { Action, combineReducers } from "redux";
import { Dispatch } from "redux";

import { startTask, endTask } from "../../helpers/asyncActions";
import { getBuilder, orthancURL } from "../../helpers/requestHelper";
import { Thunk } from "../../helpers/Thunk";
import { getPatientData } from "../patients/reducers";
import { getStudyData } from "../studies/reducers";
import { fetchContours } from "../contours/reducers";

export const updateInstances = createAction("INSTANCES/UPDATE", (instancesIds: string[]) => ({ instancesIds }));
export const updateInstanceDetails = createAction("INSTANCE_DETAILS/UPDATE",
    (pixelSpacing: string | undefined, spacingBetweenSlices: string | undefined) => ({ pixelSpacing, spacingBetweenSlices }));
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
            console.warn(response.map(f => f.IndexInSeries));
            response = response.sort((f1, f2) => f1.IndexInSeries - f2.IndexInSeries);
            console.error(response.map(f => f.IndexInSeries));
            const instancesIds: string[] = response.map(r => r.ID);
            console.log(instancesIds);
            if (instancesIds !== undefined) {
                console.warn(instancesIds);
                await dispatch(updateInstances(instancesIds));
                console.warn("update");
            }
            else {
                console.log("fetchInstances() failed");
            }
            dispatch(getPatientData());
            dispatch(getStudyData());
            dispatch(setCurrentInstanceInd(0));
            dispatch(endTask());
        }
    };

export const getDetails = (id: string): Thunk =>
    async (dispatch, getState) => {
        {
            dispatch(startTask());
            let response = await getBuilder<any>(orthancURL, "/instances/" + id + "/tags");
            if (response !== undefined) {
                const pixelSpacing: string | undefined = response["0028,0030"].Value;
                const spacingBetweenSlices: string | undefined = response["0018,0088"].Value;
                dispatch(updateInstanceDetails(pixelSpacing, spacingBetweenSlices));
            }
            dispatch(endTask());
        }
    };

export const setCurrentInstanceInd = (index: number): Thunk =>
    async (dispatch, getState) => {
        {
            dispatch(startTask());
            dispatch(updateCurrentInstance(index));
            dispatch(getDetails(getState().instancesIds[index]));

            dispatch(fetchContours("api/manualcontour/FetchByDicomIdToDTOs/" + getState().instancesIds[index]));
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
    [updateCurrentInstance.toString()](state: AppState, action) { return { ...state, ...updateCurentInctanceReducer(state, action) }; }
};
