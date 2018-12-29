import { ReducerMap, createAction } from "redux-actions";
import { Action, combineReducers } from "redux";
import { Dispatch } from "redux";

import { startTask, endTask } from "../../helpers/asyncActions";
import { getBuilder, orthancURL } from "../../helpers/requestHelper";
import { Thunk } from "../../helpers/Thunk";

export interface Serie {
    readonly id: string;
    readonly name: string;
}

export const updateSeries = createAction("SERIES/UPDATE", (series: Serie[]) => ({ series }));

export const fetchSeries = (getOpts: string): Thunk =>
    async (dispatch, getState) => {
        {
            dispatch(startTask());
            const response = await getBuilder<any>(orthancURL, getOpts);
            console.log(response);
            const series: Serie[] = response.map(r => { return { id: r.ID, name: r.MainDicomTags.SeriesDescription }; });
            if (series !== undefined) {
                dispatch(updateSeries(series));
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
            return Object.assign({}, state, { series: action.payload.series });
        default:
            return state;
    }
}

export const seriesReducers = {
    [updateSeries.toString()](state: AppState, action) { return { ...state, ...updateSeriesReducer(state, action) }; }
};
