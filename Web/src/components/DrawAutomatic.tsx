import * as React from "react";
import { apiURL, orthancURL } from "../helpers/requestHelper";
import { connect } from "react-redux";
import { Button } from "@material-ui/core";
import ChooseColorDialog from "./ChooseColorDialog";
import { Dispatch } from "redux";
import { setCurrentInstanceInd } from "../containers/instances/reducers";
import SendToApiDialog from "./SendToApiDialog";
import { sendAutomaticContour, Contour, sendPreviewContour, deletePreviewRecord, discardPreview } from "../containers/contours/reducers";

export interface DrawAutimaticProps {
    readonly instancesIds: string[];
    readonly preview: any;
    readonly currentInstanceId: number;
    readonly seriesName: string | undefined;
    readonly studyName: string | undefined;
    readonly patientName: string | undefined;
    readonly discardPreview: () => void;
    readonly setCurrentInd: (ind: number) => void;
    readonly deletePreview: (guid: string) => void;
    readonly sendAutomaticContour: (contour: Contour, centralPoints: Point[], title: string, canvasSize: Size, imgSize: Size) => void;
    readonly sendPreviewContour: (guid: string, points: Point[], color: string, canvasSize: Size, imgSize: Size) => Promise<any>;
}

export interface DrawAutimaticState {
    readonly size: Size;
    readonly imgSize: Size;
    readonly points: Point[];
    readonly pixels: Point[];
    readonly guid: string | null;
    readonly color: string;
    readonly chooseColor: boolean;
    readonly saveContourOpen: boolean;
}

export interface Size {
    readonly width: number;
    readonly height: number;
}
export interface Point {
    readonly x: number;
    readonly y: number;
}

class DrawAutimatic extends React.Component<DrawAutimaticProps, DrawAutimaticState> {
    public saveableCanvas1: any;
    public saveableCanvas2: any;
    constructor(props: DrawAutimaticProps) {
        super(props);
        this.state = {
            size: {
                width: -1,
                height: -1
            },
            imgSize: {
                width: -1,
                height: -1
            },
            points: [],
            pixels: [],
            guid: null,
            color: "#00ff00",
            chooseColor: false,
            saveContourOpen: false
        };

        this.props.preview !== undefined && this.props.deletePreview(this.props.preview.guid);
        // console.warn(this.state);
        const url = props.instancesIds.length > 0 ?
            orthancURL + "instances/" +
            this.props.instancesIds[this.props.currentInstanceId]
            + "/preview" :
            "https://imgur.com/t8wK1PH.png";
        let img = new Image();
        const fun = (w, h) => {
            this.setState({ imgSize: { width: w, height: h } });
            h = h * 1000 / w;
            w = 1000;
            if (h > 600) {
                w = w * 600 / h;
                h = 600;
            }
            this.setState({
                size: { width: w, height: h }
            });
        };
        img.onload = function () {
            // console.log(img.naturalWidth, img.naturalHeight);
            fun(img.naturalWidth, img.naturalHeight);
        };
        img.src = url;
    }

    componentWillReceiveProps(nextProps: DrawAutimaticProps) {
        const url = nextProps.instancesIds.length > 0 ?
            orthancURL + "instances/" +
            this.props.instancesIds[this.props.currentInstanceId]
            + "/preview" :
            "https://imgur.com/t8wK1PH.png";
        let img = new Image();
        const fun = (w, h) => {
            this.setState({ imgSize: { width: w, height: h } });
            h = h * 1000 / w;
            w = 1000;
            if (h > 600) {
                w = w * 600 / h;
                h = 600;
            }
            this.setState({
                size: { width: w, height: h }
            });
        };
        img.onload = function () {
            // console.warn(img.naturalWidth, img.naturalHeight);
            fun(img.naturalWidth, img.naturalHeight);
        };
        img.src = url;

        if (nextProps.preview !== undefined) {
            // console.warn("preview", nextProps.preview);
            const data = nextProps.preview;
            this.setState({
                guid: data.guid,
                pixels: data.lines[0].pixels.map(p => ({
                    x: p.x * this.state.size.width / this.state.imgSize.width,
                    y: p.y * this.state.size.height / this.state.imgSize.height
                })),
                points: data.lines[0].points.map(p => ({
                    x: p.x * this.state.size.width / this.state.imgSize.width,
                    y: p.y * this.state.size.height / this.state.imgSize.height
                }))
                // pixels: data.lines[0].pixels
            }, () => {
                // Draw pixels
                const canvas: any = document.getElementById("canvas");

                const context = canvas.getContext("2d");
                context.clearRect(0, 0, canvas.width, canvas.height);
                context.fillStyle = data.brushColor;

                this.state.points.forEach((pixel) => {
                    context.fillRect(pixel.x, pixel.y, 5, 5);
                });
                this.state.pixels.forEach((pixel) => {
                    context.fillRect(pixel.x, pixel.y, 2, 2);
                });
            });

        }

    }

    componentDidMount() {
        this.props.discardPreview();
        const canvas: any = document.getElementById("canvas");
        // console.log(canvas);
        const context = canvas.getContext("2d");

        const addPoint = (x, y) => this.setState(prev => ({ points: [...prev.points, { x, y }] }));
        const findOverlapping = (x, y) => {
            const found = this.state.points.filter(p => p.x - 5 < x && x < p.x + 5 && p.y - 5 < y && y < p.y + 5);
            return found;
        };
        const removeOverlapping = (overlapping: Point[]) => {
            const found = this.state.points.filter(p => !overlapping.find(pp => pp === p));
            this.setState({ points: found });

            context.clearRect(0, 0, canvas.width, canvas.height);
            context.fillStyle = getColor();
            found.forEach(p => context.fillRect(p.x, p.y, 5, 5));
        };

        const getColor = () => this.state.color;

        canvas.addEventListener("click", function (e) {
            const x = Math.floor(e.offsetX);
            const y = Math.floor(e.offsetY);

            const overlapping = findOverlapping(x, y);

            if (overlapping.length > 0) {
                removeOverlapping(overlapping);
            }
            else {
                addPoint(x, y);

                context.fillStyle = getColor();
                context.fillRect(x, y, 5, 5);
            }
        }, true);
    }

    render() {
        const url = this.props.instancesIds.length > 0 ?
            orthancURL + "instances/" +
            this.props.instancesIds[this.props.currentInstanceId]
            + "/preview" :
            "https://imgur.com/t8wK1PH.png";
        const bgimg = "url(" + url + ")";
        return <>
            <ChooseColorDialog
                open={this.state.chooseColor}
                initialColor={this.state.color}
                onClose={() => this.setState({ chooseColor: false })}
                onConfirm={(color: string) => this.setState({ color })}
            />
            <SendToApiDialog
                open={this.state.saveContourOpen}
                size={this.state.size}
                imgUrl={bgimg}
                contour={{
                    dicomid: this.props.instancesIds[this.props.currentInstanceId],
                    tag: "SemiAutomatic Test",
                    lines: [
                        {
                            points: this.state.points,
                            brushColor: this.state.color
                        }
                    ],
                    width: this.state.size.width,
                    height: this.state.size.height,
                    statistics: undefined
                }}
                onConfirm={(c, cp, t) => {
                    this.props.deletePreview(this.state.guid);
                    this.props.sendAutomaticContour(c, cp, t, this.state.size, this.state.imgSize);
                    const canvas: any = document.getElementById("canvas");
                    const context = canvas.getContext("2d");
                    context.clearRect(0, 0, canvas.width, canvas.height);
                    this.setState({
                        guid: null,
                        pixels: [],
                        points: []
                    });
                }}
                onClose={() => this.setState({ saveContourOpen: false })}
            />
            {this.props.instancesIds.length > 0 ?
                <><p>{this.props.patientName + "/" + this.props.studyName + "/" + this.props.seriesName}</p> <p>{(this.props.currentInstanceId + 1) + "/" + this.props.instancesIds.length}</p></> : null}
            <br />
            {<canvas id="canvas"
                width={this.state.size.width + "px"}
                height={this.state.size.height + "px"}
                style={{ backgroundImage: bgimg, backgroundSize: "cover" }}
                onWheel={(e) => {
                    e.preventDefault();
                    if (e.deltaY < 0) {
                        // console.log("scrolling up");
                        this.props.setCurrentInd(
                            this.props.currentInstanceId + 1 >= this.props.instancesIds.length ?
                                this.props.instancesIds.length - 1 :
                                this.props.currentInstanceId + 1
                        );
                    }
                    if (e.deltaY > 0) {
                        // console.log("scrolling down");
                        this.props.setCurrentInd(
                            this.props.currentInstanceId - 1 < 0 ? 0 : this.props.currentInstanceId - 1
                        );
                    }
                }}
            />}
            <br />
            <Button
                disabled={this.state.points.length < 3}
                variant="contained"
                color="primary"
                onClick={async () => {
                    // console.log("click");
                    const contour: Contour = {
                        dicomid: this.props.instancesIds[this.props.currentInstanceId],
                        tag: "SemiAutomatic Test",
                        lines: [
                            {
                                points: this.state.points,
                                brushColor: "#f00"
                            }
                        ],
                        width: this.state.size.width,
                        height: this.state.size.height,
                        statistics: undefined
                    };

                    // Send to API

                    // console.log(this.state);

                    // TODO: Refactor => make api call inside reducer
                    this.props.sendPreviewContour(this.state.guid, this.state.points, this.state.color, this.state.size, this.state.imgSize);

                }}
            >
                Preview contour
            </Button>
            <Button
                disabled={this.state.points.length < 3}
                variant="contained"
                color="primary"
                onClick={() => this.setState({ saveContourOpen: true })}
            >
                Save contour
            </Button>
            <br />
            <Button
                variant="flat"
                color="primary"
                onClick={() => this.setState({ chooseColor: true })}
            >
                Choose color
            </Button>
            <Button
                variant="flat"
                color="primary"
                onClick={() => {
                    const canvas: any = document.getElementById("canvas");
                    const context = canvas.getContext("2d");
                    context.clearRect(0, 0, canvas.width, canvas.height);
                    if (this.state.guid != null) {
                        fetch(apiURL + "api/semiautomaticpreview/delete/" + this.state.guid, {
                            mode: "cors",
                            method: "delete",
                            headers: {
                                "Accept": "application/json",
                                "Content-Type": "application/json"
                            }
                        });
                    }
                    this.setState({
                        guid: null,
                        points: []
                    });
                }}
            >
                Clear points
            </Button>
            <Button
                variant="flat"
                color="primary"
                onClick={() => {
                    const canvas: any = document.getElementById("canvas");
                    const context = canvas.getContext("2d");
                    context.clearRect(0, 0, canvas.width, canvas.height);
                    this.setState({
                        pixels: []
                    });
                    context.fillStyle = this.state.color;
                    this.state.points.forEach((pixel) => {
                        context.fillRect(pixel.x, pixel.y, 5, 5);
                    });
                }}
            >
                Clear generated contour
            </Button>
        </>;
    }
}
export default connect(
    (state: AppState) => {
        return {
            instancesIds: state.instancesIds,
            currentInstanceId: state.currentInstanceId,
            preview: state.preview,

            seriesName: state.seriesName,
            studyName: state.studyName,
            patientName: state.patientName
        };
    },
    (dispatch: Dispatch<any>) => ({
        setCurrentInd: (ind: number) => {
            dispatch(setCurrentInstanceInd(ind));
        },
        sendAutomaticContour: (contour: Contour, centralPoints: Point[], title: string, canvasSize: Size, imgSize: Size) => {
            dispatch(sendAutomaticContour("api/semiautomaticcontour/post/", { ...contour, centralPoints, tag: title }, canvasSize, imgSize));
        },
        sendPreviewContour: async (guid: string, points: Point[], color: string, canvasSize: Size, imgSize: Size) => {
            await dispatch(sendPreviewContour(guid, points, color, canvasSize, imgSize));
        },
        deletePreview: (guid: string) => {
            dispatch(deletePreviewRecord(guid));
        },
        discardPreview: () => {
            dispatch(discardPreview());
        }
    })
)(DrawAutimatic);
