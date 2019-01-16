import { createAction } from "redux-actions";

import { startTask, endTask } from "../../helpers/asyncActions";
import { getBuilder, apiURL, postBuilder } from "../../helpers/requestHelper";
import { Thunk } from "../../helpers/Thunk";
import { Size } from "../../components/DrawAutomatic";

export interface Contour {
    guid?: string;
    dicomid: string;
    tag: string;
    lines: {
        points: {
            x: number;
            y: number;
        }[];
        pixels?: {
            x: number;
            y: number;
        }[];
        brushColor: string;
    }[];
    width: number;
    height: number;
    statistics: StatisticsResult | undefined;
}

export interface StatisticsResult {
    centerOfMass: { x: number, y: number };
    histogram: number[];
    histogramMin: number;
    histogramMax: number;
    histogramMean: number;
    area: number;
    permieter: number;
    numberOfPixelsInsideContour: number;
    numberOfPixelsOfContour: number;
}

export interface ContourWithCenralPoints extends Contour {
    centralPoints: {
        x: number;
        y: number;
    }[];
}

export const updateContours = createAction("CONTOURS/UPDATE", (contours: Contour[]) => ({ contours: contours }));
export const updateContour = createAction("CONTOUR/UPDATE", (contour: Contour) => ({ contour: contour }));

export const fetchContours = (getOpts: string): Thunk =>
    async (dispatch, getState) => {
        {
            dispatch(startTask());
            let response = await getBuilder<Contour[]>(apiURL, getOpts);
            if (response !== undefined) {
                dispatch(updateContours(response));
                console.warn("update contours");
            }
            else {
                console.log("fetchContours() failed");
            }
            dispatch(endTask());
        }
    };

export const setCurrentContur = (guid: string): Thunk =>
    async (dispatch, getState) => {
        {
            dispatch(updateContour(getState().contours.find(c => c.guid === guid)));
        }
    };
export const discardCurrentContur = (): Thunk =>
    async (dispatch, getState) => {
        {
            dispatch(updateContour(undefined));
        }
    };


function updateContoursReducer(state: AppState, action) {
    switch (action.type) {
        case "CONTOURS/UPDATE":
            return Object.assign({}, state, { contours: action.payload.contours });
        default:
            return state;
    }
}
function updateContourReducer(state: AppState, action) {
    switch (action.type) {
        case "CONTOUR/UPDATE":
            return Object.assign({}, state, { selectedContour: action.payload.contour });
        default:
            return state;
    }
}

export const sendAutomaticContour = (getOpts: string, data: ContourWithCenralPoints, canvasSize: Size, imgSize: Size): Thunk =>
    async (dispatch, getState) => {
        {
            dispatch(startTask());
            console.warn(data);
            const lines = data.lines.map(line => {
                let next = line;
                next.points = next.points
                    .map(p => ({
                        x: p.x * imgSize.width / canvasSize.width,
                        y: p.y * imgSize.height / canvasSize.height
                    }))
                    .map(p => ({
                        x: parseInt(p.x.toString()),
                        y: parseInt(p.y.toString())
                    }));
                return next;
            });
            const centralPoints = data.centralPoints
                .map(p => ({
                    x: p.x * imgSize.width / canvasSize.width,
                    y: p.y * imgSize.height / canvasSize.height
                }))
                .map(p => ({
                    x: parseInt(p.x.toString()),
                    y: parseInt(p.y.toString())
                }));
            const body = { ...data, lines, centralPoints, ...imgSize };
            let response = await postBuilder<Contour>(apiURL, getOpts, body);
            if (response !== undefined) {
                dispatch(updateContour(response));
                console.warn("update contour");
            }
            else {
                console.log("sendAutomaticContour() failed");
            }
            dispatch(endTask());
        }
    };
export const sendManualContour = (getOpts: string, data: ContourWithCenralPoints, canvasSize: Size, imgSize: Size): Thunk =>
    async (dispatch, getState) => {
        {
            dispatch(startTask());
            console.warn(data);
            const lines = data.lines.map(line => {
                let next = line;
                next.points = next.points
                    .map(p => ({
                        x: p.x * imgSize.width / canvasSize.width,
                        y: p.y * imgSize.height / canvasSize.height
                    }))
                    .map(p => ({
                        x: parseInt(p.x.toString()),
                        y: parseInt(p.y.toString())
                    }));
                // add closing line
                const l = next.points.length;
                next.points[l] = { x: next.points[0].x, y: next.points[l - 1].y };
                return next;
            });
            const centralPoints = data.centralPoints
                .map(p => ({
                    x: p.x * imgSize.width / canvasSize.width,
                    y: p.y * imgSize.height / canvasSize.height
                }))
                .map(p => ({
                    x: parseInt(p.x.toString()),
                    y: parseInt(p.y.toString())
                }));
            const body = { ...data, lines, centralPoints, ...imgSize };
            let response = await postBuilder<Contour>(apiURL, getOpts, body);
            if (response !== undefined) {
                console.log("sendManualContour() succeded");
            }
            else {
                console.log("sendManualContour() failed");
            }
            dispatch(endTask());
        }
    };


export const contoursReducers = {
    [updateContours.toString()](state: AppState, action) { return { ...state, ...updateContoursReducer(state, action) }; },
    [updateContour.toString()](state: AppState, action) { return { ...state, ...updateContourReducer(state, action) }; }
};
