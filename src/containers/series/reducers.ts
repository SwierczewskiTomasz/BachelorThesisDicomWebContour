import { ReducerMap, createAction } from "redux-actions";
import { Action, combineReducers } from "redux";
import { Dispatch } from "redux";

import { startTask, endTask } from "../../helpers/asyncActions";
import { getBuilder, orthancURL } from "../../helpers/requestHelper";
import { Thunk } from "../../helpers/Thunk";


export const updateSeries = createAction("SERIES/UPDATE", (seriesIds: string[]) => ({seriesIds}));

export const fetchSeries = (): Thunk =>
    async (dispatch, getState) => {
    {
        dispatch(startTask());
        let seriesIds = await getBuilder<string[]>(orthancURL, "series");
        console.log(seriesIds);
        if (seriesIds !== undefined) {
            dispatch(updateSeries(seriesIds));
            console.warn("update");
        }
        else {
            console.log("fetchSeries() failed");
        }
        dispatch(endTask());
    }
};

function updateSeriesReducer(state: AppState, action) {
    switch (action.type) {
      case "SERIES/UPDATE":
        return Object.assign({}, state, {seriesIds: action.payload.seriesIds});
      default:
        return state;
    }
}

export const seriesReducers = {
    [updateSeries.toString()](state: AppState, action) { return { ...state, ...updateSeriesReducer(state, action) }; }
};
