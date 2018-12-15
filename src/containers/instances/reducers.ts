import { ReducerMap, createAction } from "redux-actions";
import { Action, combineReducers } from "redux";
import { Dispatch } from "redux";

import { startTask, endTask } from "../../helpers/asyncActions";
import { getBuilder, orthancURL } from "../../helpers/requestHelper";
import { Thunk } from "../../helpers/Thunk";

export const updateInstances = createAction("INSTANCES/UPDATE", (instancesIds: string[]) => ({ instancesIds }));

export const fetchInstances = (getOpts: string): Thunk =>
    async (dispatch, getState) => {
        {
            dispatch(startTask());
            let response = await getBuilder<any>(orthancURL, getOpts);
            console.warn(response);
            const instancesIds: string[] = response.map(r => r.ID);
            console.log(instancesIds);
            if (instancesIds !== undefined) {
                dispatch(updateInstances(instancesIds));
                console.warn("update");
            }
            else {
                console.log("fetchInstances() failed");
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

export const instancesReducers = {
    [updateInstances.toString()](state: AppState, action) { return { ...state, ...updateInstancesReducer(state, action) }; }
};
