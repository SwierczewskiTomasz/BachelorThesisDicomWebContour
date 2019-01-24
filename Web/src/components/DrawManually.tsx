import * as React from "react";
import { orthancURL } from "../helpers/requestHelper";
import { connect } from "react-redux";
import CanvasDraw from "react-canvas-draw";
import { Button } from "@material-ui/core";
import ChooseColorDialog from "./ChooseColorDialog";
import { Dispatch } from "redux";
import { setCurrentInstanceInd } from "../containers/instances/reducers";
import { Contour, sendManualContour } from "../containers/contours/reducers";
import { Size, Point } from "./DrawAutomatic";
import SendToApiDialog from "./SendToApiDialog";

export interface DrawManuallyProps {
    readonly instancesIds: string[];
    readonly currentInstanceId: number;
    readonly selectedContour: Contour;
    readonly seriesName: string | undefined;
    readonly studyName: string | undefined;
    readonly patientName: string | undefined;
    readonly setCurrentInd: (ind: number) => void;
    readonly sendManualContour: (contour: Contour, centralPoints: Point[], title: string, canvasSize: Size, imgSize: Size) => void;
}

export interface DrawManuallyState {
    readonly size: Size;
    readonly imgSize: Size;
    readonly reload: boolean;
    readonly color: string;
    readonly chooseColor: boolean;
    readonly saveContourOpen: boolean;
    readonly contour?: Contour;
}

class DrawManually extends React.Component<DrawManuallyProps, DrawManuallyState> {
    public saveableCanvas1: any;
    public saveableCanvas2: any;
    constructor(props: DrawManuallyProps) {
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
            reload: true,
            color: "#ff0000",
            chooseColor: false,
            saveContourOpen: false
        };
        console.warn("state", this.state);
        const url = props.instancesIds.length > 0 ?
            orthancURL + "instances/" +
            this.props.instancesIds[this.props.currentInstanceId]
            + "/preview" :
            "https://imgur.com/t8wK1PH.png";
        let img = new Image();
        const fun = (w, h) => {
            this.setState({ imgSize: { width: w, height: h } });
            h = (h * 1000 / w);
            w = 1000;
            if (h > 600) {
                w = w * 600 / h;
                h = 600;
            }
            this.setState(prev => ({ size: { width: w, height: h }, reload: !prev.reload }));
        };

        img.onload = function () {
            console.warn(img.naturalWidth, img.naturalHeight);
            fun(img.naturalWidth, img.naturalHeight);
        };
        img.src = url;
    }

    componentWillReceiveProps(nextProps: DrawManuallyProps) {
        const url = nextProps.instancesIds.length > 0 ?
            orthancURL + "instances/" +
            this.props.instancesIds[this.props.currentInstanceId]
            + "/preview" :
            "https://imgur.com/t8wK1PH.png";
        let img = new Image();
        const fun = (w, h) => {
            this.setState({ imgSize: { width: w, height: h } });
            h = (h * 1000 / w);
            w = 1000;
            if (h > 600) {
                w = w * 600 / h;
                h = 600;
            }
            this.setState(prev => ({ size: { width: w, height: h }, reload: !prev.reload }));
        };

        img.onload = function () {
            console.warn(img.naturalWidth, img.naturalHeight);
            fun(img.naturalWidth, img.naturalHeight);
        };
        img.src = url;

        this.setState(prev => { return { reload: !prev.reload }; }, () => {
            if (this.props.selectedContour !== undefined && this.props.selectedContour !== nextProps.selectedContour) {
                const { guid, dicomid, tag, ...data } = nextProps.selectedContour;
                this.state.reload ? this.saveableCanvas1.loadSaveData(data, true) : this.saveableCanvas2.loadSaveData(data, true);
            }
        });
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
            this.props.instancesIds[this.props.currentInstanceId]
            + "/preview" :
            "https://imgur.com/t8wK1PH.png";
        const bgimg = "url(" + url + ")";
        return <>
            {/* <Button
                variant="contained"
                color="primary"
                onClick={() => this.setState(prev => ({ currentInstanceId: prev.currentInstanceId + 1 }))}
            >
                +
            </Button> */}
            {this.props.instancesIds.length > 0 ?
                <><p>{this.props.patientName + "/" + this.props.studyName + "/" + this.props.seriesName}</p> <p>{(this.props.currentInstanceId + 1) + "/" + this.props.instancesIds.length}</p></> : null}
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
                contour={this.state.contour}
                onConfirm={(c, cp, t) => {
                    const toSend = { ...c };
                    toSend.lines = [toSend.lines[0]];
                    this.props.sendManualContour(toSend, cp, t, this.state.size, this.state.imgSize);
                    this.state.reload ?
                        this.saveableCanvas1.clear() :
                        this.saveableCanvas2.clear();
                    if (c.lines.length > 1) {
                        const { dicomid, ...contour } = c;
                        contour.lines = c.lines.filter((l, i) => i > 0);
                        console.warn(contour);
                        this.state.reload ?
                            this.saveableCanvas1.loadSaveData(JSON.stringify(contour), true) :
                            this.saveableCanvas2.loadSaveData(JSON.stringify(contour), true);
                    }
                }}
                onClose={() => this.setState({ saveContourOpen: false })}
            />
            {this.state.reload && <div
                onWheel={(e) => {
                    e.preventDefault();
                    if (e.deltaY < 0) {
                        console.log("div1 scrolling up");
                        this.setState(prev => ({
                            reload: !prev.reload
                        }), () => this.props.setCurrentInd(
                            this.props.currentInstanceId + 1 >= this.props.instancesIds.length ?
                                this.props.instancesIds.length - 1 :
                                this.props.currentInstanceId + 1
                        ));
                    }
                    if (e.deltaY > 0) {
                        console.log("div scrolling down");
                        this.setState(prev => ({
                            reload: !prev.reload
                        }), () => this.props.setCurrentInd(
                            this.props.currentInstanceId - 1 < 0 ? 0 : this.props.currentInstanceId - 1
                        ));
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
                    e.preventDefault();
                    if (e.deltaY < 0) {
                        console.log("div scrolling up");
                        this.setState(prev => ({
                            reload: !prev.reload
                        }), () => this.props.setCurrentInd(
                            this.props.currentInstanceId + 1 >= this.props.instancesIds.length ?
                                this.props.instancesIds.length - 1 :
                                this.props.currentInstanceId + 1
                        ));
                    }
                    if (e.deltaY > 0) {
                        console.log("div scrolling down");
                        this.setState(prev => ({
                            reload: !prev.reload
                        }), () => this.props.setCurrentInd(
                            this.props.currentInstanceId - 1 < 0 ? 0 : this.props.currentInstanceId - 1
                        ));
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
            <Button
                variant="contained"
                color="primary"
                onClick={() => {
                    console.log("click");
                    const data = JSON.parse(this.state.reload ? this.saveableCanvas1.getSaveData() : this.saveableCanvas2.getSaveData());
                    // localStorage.setItem("savedDrawing", data);
                    console.warn(data);
                    this.setState({
                        contour: {
                            ...data,
                            dicomid: this.props.instancesIds[this.props.currentInstanceId]
                        }
                    });
                    this.setState({ saveContourOpen: true });
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
            <Button
                variant="flat"
                color="primary"
                onClick={() => {
                    this.state.reload ?
                        this.saveableCanvas1.clear() :
                        this.saveableCanvas2.clear();
                }}
            >
                Clear image
            </Button>
        </>;
    }
}
export default connect(
    (state: AppState) => {
        return {
            instancesIds: state.instancesIds,
            currentInstanceId: state.currentInstanceId,
            selectedContour: state.selectedContour,

            seriesName: state.seriesName,
            studyName: state.studyName,
            patientName: state.patientName
        };
    },
    (dispatch: Dispatch<any>) => ({
        setCurrentInd: (ind: number) => {
            dispatch(setCurrentInstanceInd(ind));
        },
        sendManualContour: (contour: Contour, centralPoints: Point[], title: string, canvasSize: Size, imgSize: Size) => {
            dispatch(sendManualContour("api/manualcontour/post/", { ...contour, centralPoints, tag: title }, canvasSize, imgSize));
        }
    })
)(DrawManually);
