import * as React from "react";
import { orthancURL } from "../helpers/requestHelper";
import { connect } from "react-redux";
import CanvasDraw from "react-canvas-draw";
import { Button } from "@material-ui/core";
import ChooseColorDialog from "./ChooseColorDialog";

export interface DrawManuallyProps {
    readonly instancesIds: string[];
}

export interface DrawManuallyState {
    readonly currentInstanceId: number;
    readonly size: Size;
    readonly reload: boolean;
    readonly color: string;
    readonly chooseColor: boolean;
}

interface Size {
    readonly width: number;
    readonly height: number;
}

class DrawManually extends React.Component<DrawManuallyProps, DrawManuallyState> {
    public saveableCanvas1: any;
    public saveableCanvas2: any;
    constructor(props: DrawManuallyProps) {
        super(props);
        this.state = {
            currentInstanceId: 0,
            size: {
                width: -1,
                height: -1
            },
            reload: true,
            color: "#ff0000",
            chooseColor: false
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

    componentWillReceiveProps(nextProps: DrawManuallyProps) {
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
    render() {
        const canvasProps = {
            loadTimeOffset: 5,
            lazyRadius: 0,
            brushRadius: 0,
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
        return <>
            {/* <Button
                variant="contained"
                color="primary"
                onClick={() => this.setState(prev => ({ currentInstanceId: prev.currentInstanceId + 1 }))}
            >
                +
            </Button> */}

            <ChooseColorDialog
                open={this.state.chooseColor}
                initialColor={this.state.color}
                onClose={() => this.setState({ chooseColor: false })}
                onConfirm={(color: string) => this.setState({ color })}
            />

            {this.state.reload && <div
                onWheel={(e) => {
                    if (e.deltaY < 0) {
                        console.log("div1 scrolling up");
                        this.setState(prev => ({
                            currentInstanceId: prev.currentInstanceId + 1 >= this.props.instancesIds.length ?
                                this.props.instancesIds.length - 1 :
                                prev.currentInstanceId + 1,
                            reload: !prev.reload
                        }));
                    }
                    if (e.deltaY > 0) {
                        console.log("div scrolling down");
                        this.setState(prev => ({
                            currentInstanceId: prev.currentInstanceId - 1 < 0 ? 0 : prev.currentInstanceId - 1,
                            reload: !prev.reload
                        }));
                    }
                }}
                onClick={(e) => { console.warn(e); }}
            >
                <CanvasDraw
                    ref={canvasDraw => (this.saveableCanvas1 = canvasDraw)}
                    {...canvasProps}
                    brushColor={this.state.color}
                    imgSrc={url}
                    canvasWidth={this.state.size.width}
                    canvasHeight={this.state.size.height}
                    style={{ margin: "0 auto", cursor: "crosshair" }}
                />
            </div>}
            {!this.state.reload && <div
                onWheel={(e) => {
                    if (e.deltaY < 0) {
                        console.log("div scrolling up");
                        this.setState(prev => ({
                            currentInstanceId: prev.currentInstanceId + 1 >= this.props.instancesIds.length ?
                                this.props.instancesIds.length - 1 :
                                prev.currentInstanceId + 1,
                            reload: !prev.reload
                        }));
                    }
                    if (e.deltaY > 0) {
                        console.log("div scrolling down");
                        this.setState(prev => ({
                            currentInstanceId: prev.currentInstanceId - 1 < 0 ? 0 : prev.currentInstanceId - 1,
                            reload: !prev.reload
                        }));
                    }
                }}>
                <CanvasDraw
                    ref={canvasDraw => (this.saveableCanvas2 = canvasDraw)}
                    {...canvasProps}
                    brushColor={this.state.color}
                    imgSrc={url}
                    canvasWidth={this.state.size.width}
                    canvasHeight={this.state.size.height}
                    style={{ margin: "0 auto", cursor: "crosshair" }}

                />
            </div>}
            {/* <img style={{ margin: "0 auto" }}
                src={
                    this.props.instancesIds.length > 0 ?
                        orthancURL + "instances/" +
                        this.props.instancesIds[this.state.currentInstanceId]
                        + "/preview" :
                        "https://http.cat/404"
                }
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
            /> */}
            <Button
                variant="contained"
                color="primary"
                onClick={() => {
                    console.log("click");
                    const data = this.state.reload ? this.saveableCanvas1.getSaveData() : this.saveableCanvas2.getSaveData();
                    // localStorage.setItem("savedDrawing", data);
                    console.warn(data);
                    // Send to API
                    fetch("https://localhost:5001/api/manualcontour/post/", {
                        mode: "cors",
                        method: "post",
                        headers: {
                            "Accept": "application/json",
                            "Content-Type": "application/json"
                        },
                        body: JSON.stringify({
                            dicomid: this.props.instancesIds[this.state.currentInstanceId],
                            ...data
                        })
                    });
                }}
            >
                Save Contour
            </Button>
            <Button
                variant="flat"
                color="primary"
                onClick={() => this.setState({ chooseColor: true })}
            >
                Choose color
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
)(DrawManually);
