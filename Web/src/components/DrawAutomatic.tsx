import * as React from "react";
import { orthancURL } from "../helpers/requestHelper";
import { connect } from "react-redux";
import CanvasDraw from "react-canvas-draw";
import { Button } from "@material-ui/core";
import { defaultCipherList } from "constants";
import ChooseColorDialog from "./ChooseColorDialog";
import { Dispatch } from "redux";
import { setCurrentInstanceInd } from "../containers/instances/reducers";

export interface DrawAutimaticProps {
    readonly instancesIds: string[];
    readonly currentInstanceId: number;
    readonly setCurrentInd: (ind: number) => void;
}

export interface DrawAutimaticState {
    readonly size: Size;
    readonly points: Point[];
    readonly pixels: Point[];
    readonly reload: boolean;
    readonly guid: string;
    readonly color: string;
    readonly chooseColor: boolean;
}

interface Size {
    readonly width: number;
    readonly height: number;
}
interface Point {
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
            reload: true,
            points: [],
            pixels: [],
            guid: null,
            color: "#00ff00",
            chooseColor: false
        };
        console.warn(this.state);
        const url = props.instancesIds.length > 0 ?
            orthancURL + "instances/" +
            this.props.instancesIds[this.props.currentInstanceId]
            + "/preview" :
            "https://http.cat/404";
        let img = new Image();
        const fun = (w, h) => this.setState(prev => { return { size: { width: w, height: h }, reload: !prev.reload }; });
        img.onload = function () {
            console.log(img.naturalWidth, img.naturalHeight);
            fun(img.naturalWidth, img.naturalHeight);
        };
        img.src = url;
    }

    componentWillReceiveProps(nextProps: DrawAutimaticProps) {
        const url = nextProps.instancesIds.length > 0 ?
            orthancURL + "instances/" +
            this.props.instancesIds[this.props.currentInstanceId]
            + "/preview" :
            "https://http.cat/404";
        let img = new Image();
        const fun = (w, h) => this.setState({ size: { width: w, height: h } });
        img.onload = function () {
            console.warn(img.naturalWidth, img.naturalHeight);
            fun(img.naturalWidth, img.naturalHeight);
        };
        img.src = url;
        this.setState(prev => { return { reload: !prev.reload }; });

    }

    componentDidMount() {

        const canvas: any = document.getElementById("canvas");

        const context = canvas.getContext("2d");

        // Background
        // context.fillStyle = "#0f0";
        // context.fillRect(0, 0, canvas.width, canvas.height);

        const addPoint = (x, y) => this.setState(prev => { return { points: [...prev.points, { x, y }] }; });
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
            "https://http.cat/404";
        const bgimg = "url(" + url + ")";
        return <>
            <ChooseColorDialog
                open={this.state.chooseColor}
                initialColor={this.state.color}
                onClose={() => this.setState({ chooseColor: false })}
                onConfirm={(color: string) => this.setState({ color })}
            />
            {this.props.instancesIds.length > 0 ? (this.props.currentInstanceId + 1) + "/" + this.props.instancesIds.length : null}
            <br />
            {<canvas id="canvas"
                width={this.state.size.width + "px"}
                height={this.state.size.height + "px"}
                style={{ background: bgimg }}
                onWheel={(e) => {
                    if (e.deltaY < 0) {
                        console.log("scrolling up");
                        this.setState(prev => ({
                            reload: !prev.reload
                        }), () => this.props.setCurrentInd(
                            this.props.currentInstanceId + 1 >= this.props.instancesIds.length ?
                                this.props.instancesIds.length - 1 :
                                this.props.currentInstanceId + 1
                        ));
                    }
                    if (e.deltaY > 0) {
                        console.log("scrolling down");
                        this.setState(prev => ({
                            reload: !prev.reload
                        }), () => this.props.setCurrentInd(
                            this.props.currentInstanceId - 1 < 0 ? 0 : this.props.currentInstanceId - 1
                        ));
                    }
                }}
            />}
            <br />
            {/* <img style={{ margin: "0 auto" }}
                src={
                    this.props.instancesIds.length > 0 ?
                        orthancURL + "instances/" +
                        this.props.instancesIds[this.state.currentInstanceId]
                        + "/preview" :
                        "https://http.cat/404"
                }
            /> */}
            <Button
                variant="contained"
                color="primary"
                onClick={() => {
                    console.log("click");
                    localStorage.setItem(
                        "savedDrawing",
                        JSON.stringify(this.state.points)
                    );
                    console.warn(
                        JSON.stringify(this.state.points)
                    );
                    console.log(JSON.stringify({
                        dicomid: this.props.instancesIds[this.props.currentInstanceId],
                        tag: "SemiAutomatic Test",
                        lines: [
                            {
                                points: this.state.points,
                                brushColor: "#f00"
                            }
                        ],
                        width: this.state.size.width,
                        height: this.state.size.height
                    }));

                    // Send to API
                    fetch("https://localhost:5001/api/semiautomaticcontour/post/", {
                        mode: "cors",
                        method: "post",
                        headers: {
                            "Accept": "application/json",
                            "Content-Type": "application/json"
                        },
                        body: JSON.stringify({
                            dicomid: this.props.instancesIds[this.props.currentInstanceId],
                            tag: "SemiAutomatic Test",
                            lines: [
                                {
                                    points: this.state.points,
                                    brushColor: "#f00"
                                }
                            ],
                            width: this.state.size.width,
                            height: this.state.size.height
                        })
                    }).then(response => {
                        console.log(response);
                        return response.json();
                    }).then(data => {
                        console.log(data);
                        this.setState(prev => ({
                            guid: data.guid,
                            pixels: data.lines[0].pixels,
                            reload: !prev.reload
                        }));
                    }).then(prev => {
                        console.log(this.state.guid);
                        console.log(this.state.pixels);
                    });

                    // Draw pixels
                    const canvas: any = document.getElementById("canvas");

                    const context = canvas.getContext("2d");
                    context.fillStyle = "#F00";

                    this.state.pixels.forEach((pixel) => {
                        context.fillRect(pixel.x, pixel.y, 2, 2);
                    });
                }}
            >
                Generate contour
            </Button>
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
                    this.setState({ points: [] });
                }}
            >
                Clear points
            </Button>
        </>;
    }
}
export default connect(
    (state: AppState) => {
        return {
            instancesIds: state.instancesIds,
            currentInstanceId: state.currentInstanceId
        };
    },
    (dispatch: Dispatch<any>) => ({
        setCurrentInd: (ind: number) => {
            dispatch(setCurrentInstanceInd(ind));
        },
    })
)(DrawAutimatic);
