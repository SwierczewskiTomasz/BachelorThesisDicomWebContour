import * as React from "react";
import { orthancURL } from "../helpers/requestHelper";
import { connect } from "react-redux";
import CanvasDraw from "react-canvas-draw";
import { Button } from "@material-ui/core";

export interface DisplayImageProps {
    readonly instancesIds: string[];
}

export interface DisplayImageState {
    readonly currentInstanceId: number;
    readonly lastScrollTop: number;
    readonly size: Size;
    readonly url: string;
    readonly reload: boolean;
}

interface Size {
    readonly width: number;
    readonly height: number;
}

class DisplayImage extends React.Component<DisplayImageProps, DisplayImageState> {
    public saveableCanvas1: any;
    public saveableCanvas2: any;
    constructor(props: DisplayImageProps) {
        super(props);
        this.state = {
            currentInstanceId: 0,
            lastScrollTop: 0,
            size: {
                width: -1,
                height: -1
            },
            url: "",
            reload: true
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

    componentWillReceiveProps(nextProps: DisplayImageProps) {
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
        return <>
            {/* <Button
                variant="contained"
                color="primary"
                onClick={() => this.setState(prev => ({ currentInstanceId: prev.currentInstanceId + 1 }))}
            >
                +
            </Button> */}
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
                }}><CanvasDraw
                    ref={canvasDraw => (this.saveableCanvas1 = canvasDraw)}
                    {...canvasProps}
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
                    localStorage.setItem(
                        "savedDrawing",
                        this.state.reload ? this.saveableCanvas1.getSaveData() : this.saveableCanvas2.getSaveData()
                    );
                    console.warn(
                        this.state.reload ? this.saveableCanvas1.getSaveData() : this.saveableCanvas2.getSaveData());
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
)(DisplayImage);
