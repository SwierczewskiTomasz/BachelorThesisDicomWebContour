import { createAction } from "redux-actions";

import { startTask, endTask } from "../../helpers/asyncActions";
import { getBuilder, apiURL, postBuilder } from "../../helpers/requestHelper";
import { Thunk } from "../../helpers/Thunk";
import { Size, Point } from "../../components/DrawAutomatic";

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

export const updateSelectedContourGuids = createAction("SELECTED_CONTOURS/UPDATE", (contourGuids: string[]) => ({ selectedContourGuids: contourGuids }));
export const updateContours = createAction("CONTOURS/UPDATE", (contours: Contour[]) => ({ contours: contours }));
export const updateContour = createAction("CONTOUR/UPDATE", (contour: Contour) => ({ contour: contour }));
export const updatePreview = createAction("PREVIEW/UPDATE", (preview: any) => ({ preview: preview }));

export const fetchContours = (getOpts: string, getOpts2: string): Thunk =>
    async (dispatch, getState) => {
        {
            dispatch(startTask());
            let response = await getBuilder<Contour[]>(apiURL, getOpts);
            let response2 = await getBuilder<Contour[]>(apiURL, getOpts2);
            if (response !== undefined && response2 !== undefined) {
                dispatch(updateContours([...response, ...response2]));
                console.warn("update contours");
            } else if (response !== undefined) {
                dispatch(updateContours(response));
                console.warn(getOpts2, "failed");
            } else if (response2 !== undefined) {
                dispatch(updateContours(response2));
                console.warn(getOpts, "failed");
            }
            else {
                console.log("fetchContours() failed");
            }
            dispatch(endTask());
        }
    };

export const setCurrentContour = (guid: string): Thunk =>
    async (dispatch, getState) => {
        {
            dispatch(updateContour(getState().contours.find(c => c.guid === guid)));
        }
    };

export const discardCurrentContour = (): Thunk =>
    async (dispatch, getState) => {
        {
            dispatch(updateContour(undefined));
        }
    };

export const addSelectedContour = (guid: string): Thunk =>
    async (dispatch, getState) => {
        {
            const contourGuids = getState().selectedContourGuids;
            dispatch(updateSelectedContourGuids([...contourGuids, guid]));
        }
    };

export const removeSelectedContour = (guid: string): Thunk =>
    async (dispatch, getState) => {
        {
            const contourGuids = getState().selectedContourGuids.filter(cg => cg !== guid);
            dispatch(updateSelectedContourGuids(contourGuids));
        }
    };

export const removeAllSelectedContour = (): Thunk =>
    async (dispatch, getState) => {
        {
            dispatch(updateSelectedContourGuids([]));
        }
    };

export const discardPreview = (): Thunk =>
    async (dispatch, getState) => {
        {
            dispatch(updatePreview(undefined));
        }
    };

function updateSelectedContourGuidsReducer(state: AppState, action) {
    switch (action.type) {
        case "SELECTED_CONTOURS/UPDATE":
            return Object.assign({}, state, { selectedContourGuids: action.payload.selectedContourGuids });
        default:
            return state;
    }
}

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


function updatePreviewReducer(state: AppState, action) {
    switch (action.type) {
        case "PREVIEW/UPDATE":
            return Object.assign({}, state, { preview: action.payload.preview });
        default:
            return state;
    }
}

export const sendPreviewContour = (guid: string, points: Point[], color: string, canvasSize: Size, imgSize: Size): Thunk =>

    async (dispatch, getState) => {
        dispatch(startTask());
        const state = getState();
        if (guid != null) {
            fetch("https://localhost:5001/" + "api/semiautomaticpreview/delete/" + guid, {
                mode: "cors",
                method: "delete",
                headers: {
                    "Accept": "application/json",
                    "Content-Type": "application/json"
                }
            });
        }
        const response = await fetch("https://localhost:5001/" + (guid == null ? "api/semiautomaticpreview/post/" : "api/semiautomaticpreview/put/"), {
            mode: "cors",
            method: guid == null ? "post" : "put",
            headers: {
                "Accept": "application/json",
                "Content-Type": "application/json"
            },
            body: (guid != null ? JSON.stringify({
                guid: guid,
                dicomid: state.instancesIds[state.currentInstanceId],
                tag: "SemiAutomatic Test",
                lines: [
                    {
                        points: points
                            .map(p => ({
                                x: (p.x * imgSize.width) / canvasSize.width,
                                y: (p.y * imgSize.height) / canvasSize.height
                            }))
                            .map(p => ({
                                x: parseInt(p.x.toString()),
                                y: parseInt(p.y.toString())
                            })),
                        brushColor: color
                    }
                ],
                width: canvasSize.width,
                height: canvasSize.height
            }) :
                JSON.stringify({
                    dicomid: state.instancesIds[state.currentInstanceId],
                    tag: "SemiAutomatic Test",
                    lines: [
                        {
                            points: points
                                .map(p => ({
                                    x: (p.x * imgSize.width) / canvasSize.width,
                                    y: (p.y * imgSize.height) / canvasSize.height
                                }))
                                .map(p => ({
                                    x: parseInt(p.x.toString()),
                                    y: parseInt(p.y.toString())
                                })),
                            brushColor: color
                        }
                    ],
                    width: parseInt(canvasSize.width.toString()),
                    height: parseInt(canvasSize.height.toString())
                }))
        });
        console.warn(response);
        const responseDeserialized = await response.json();
        console.warn(responseDeserialized);
        dispatch(updatePreview(responseDeserialized));
        dispatch(endTask());
    };

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
            const imgSize2 = { width: parseInt(imgSize.width.toString()), height: parseInt(imgSize.height.toString()) };
            const body = { ...data, lines, centralPoints, ...imgSize2 };
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
            const imgSize2 = { width: parseInt(imgSize.width.toString()), height: parseInt(imgSize.height.toString()) };
            const body = { ...data, lines, centralPoints, ...imgSize2 };
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
    [updateSelectedContourGuids.toString()](state: AppState, action) { return { ...state, ...updateSelectedContourGuidsReducer(state, action) }; },
    [updateContours.toString()](state: AppState, action) { return { ...state, ...updateContoursReducer(state, action) }; },
    [updateContour.toString()](state: AppState, action) { return { ...state, ...updateContourReducer(state, action) }; },
    [updatePreview.toString()](state: AppState, action) { return { ...state, ...updatePreviewReducer(state, action) }; }
};
