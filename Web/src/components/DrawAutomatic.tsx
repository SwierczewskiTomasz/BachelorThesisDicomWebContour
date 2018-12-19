import * as React from "react";
import { orthancURL } from "../helpers/requestHelper";
import { connect } from "react-redux";
import CanvasDraw from "react-canvas-draw";
import { Button } from "@material-ui/core";

export interface DrawAutimaticProps {
    readonly instancesIds: string[];
}

export interface DrawAutimaticState {
    readonly currentInstanceId: number;
    readonly size: Size;
    readonly points: Point[];
    readonly reload: boolean;
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
            currentInstanceId: 0,
            size: {
                width: -1,
                height: -1
            },
            reload: true,
            points: []
        };
        console.warn(this.state);
        const url = props.instancesIds.length > 0 ?
            orthancURL + "instances/" +
            this.props.instancesIds[this.state.currentInstanceId]
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
            this.props.instancesIds[this.state.currentInstanceId]
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

        const f = (x, y) => this.setState(prev => { return { points: [...prev.points, { x, y }] }; });

        canvas.addEventListener("click", function (e) {
            const x = Math.floor(e.layerX);
            const y = Math.floor(e.layerY);

            f(x, y);

            // Zoomed in red 'square'
            context.fillStyle = "#F0F";
            context.fillRect(x, y, 5, 5);
        }, true);

    }

    render() {
        const canvasProps = {
            loadTimeOffset: 5,
            lazyRadius: 0,
            brushRadius: 0,
            brushColor: "#f00",
            catenaryColor: "transparent",
            gridColor: "rgba(150,150,150,0.17)",
            hideGrid: true,
            disabled: false,
            saveData: null,
            immediateLoading: false
        };
        const url = this.props.instancesIds.length > 0 ?
            orthancURL + "instances/" +
            this.props.instancesIds[this.state.currentInstanceId]
            + "/preview" :
            "https://http.cat/404";
        const bgimg = "url(" + url + ")";
        return <>
            {<canvas id="canvas"
                width={this.state.size.width + "px"}
                height={this.state.size.height + "px"}
                style={{ background: bgimg }}
                onWheel={(e) => {
                    if (e.deltaY < 0) {
                        console.log("scrolling up");
                        this.setState(prev => ({
                            currentInstanceId: prev.currentInstanceId + 1 >= this.props.instancesIds.length ?
                                this.props.instancesIds.length - 1 :
                                prev.currentInstanceId + 1,
                            reload: !prev.reload
                        }));
                    }
                    if (e.deltaY > 0) {
                        console.log("scrolling down");
                        this.setState(prev => ({
                            currentInstanceId: prev.currentInstanceId - 1 < 0 ? 0 : prev.currentInstanceId - 1,
                            reload: !prev.reload
                        }));
                    }
                }}
            />}
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
                }}
            >
                GetSavedData
            </Button>
        </>;
    }
}
export default connect(
    (state: AppState) => {
        return {
            instancesIds: state.instancesIds,
        };
    }
)(DrawAutimatic);
