import * as React from "react";
import { Button, Dialog, DialogContent, DialogActions, DialogTitle, Grid } from "@material-ui/core";
import { Contour } from "../containers/contours/reducers";
import { Size } from "./DrawAutomatic";
import { orthancURL } from "../helpers/requestHelper";
import { connect } from "react-redux";
import { Dispatch } from "redux";
import { setCurrentInstanceInd } from "../containers/instances/reducers";

interface PreviewSavedContoursProps {
    readonly contours: Contour[];
    readonly currentImageId: string;
    readonly instancesIds: string[];
    readonly currentInstanceId: number;
    readonly setCurrentInd: (index: number) => void;
}
interface PreviewSavedContoursState {
    readonly size: Size;
}

const strong = {
    fontWeight: 700
};

class PreviewSavedContours extends React.Component<PreviewSavedContoursProps, PreviewSavedContoursState> {
    constructor(props: PreviewSavedContoursProps) {
        super(props);
        this.state = {
            size: {
                width: 1,
                height: 1
            }
        };
    }
    componentDidMount() {
        this.redraw(this.props);
    }

    componentWillReceiveProps(props: PreviewSavedContoursProps) {
        this.redraw(props);
    }

    redraw(props: PreviewSavedContoursProps) {
        const url = this.props.instancesIds.length > 0 ?
            orthancURL + "instances/" + this.props.currentImageId + "/preview" :
            "https://http.cat/404";
        let img = new Image();
        const fun = (w, h) => {
            h = h * 1000 / w;
            w = 1000;
            if (h > 600) {
                w = w * 600 / h;
                h = 600;
            }
            console.log(w, h);
            this.setState({
                size: { width: w, height: h }
            });
        };
        img.onload = function () {
            fun(img.naturalWidth, img.naturalHeight);
        };
        img.src = url;
        const canvas: any = document.getElementById("canvas-preview");
        if (canvas === null || canvas === undefined) return;
        console.log(canvas);
        const context = canvas.getContext("2d");

        context.clearRect(0, 0, canvas.width, canvas.height);

        if (props.contours.length === 0) return;

        props.contours.forEach(c => c.lines.forEach(l => {
            if (l.pixels !== undefined) {
                context.fillStyle = l.brushColor;
                for (let i = 0; i < l.points.length; i++) {
                    context.fillRect(
                        l.points[i].x * this.state.size.width / c.width,
                        l.points[i].y * this.state.size.height / c.height,
                        5, 5);
                }
                for (let i = 0; i < l.pixels.length; i++) {
                    const p = l.pixels[i];
                    context.fillRect(p.x * this.state.size.width / c.width,
                        p.y * this.state.size.height / c.height,
                        2, 2);
                }
            }
            else {
                context.strokeStyle = l.brushColor;
                context.beginPath();
                context.moveTo(l.points[0].x * this.state.size.width / c.width, l.points[0].y * this.state.size.height / c.height);
                for (let i = 1; i < l.points.length; i++) {
                    context.lineTo(l.points[i].x * this.state.size.width / c.width, l.points[i].y * this.state.size.height / c.height);
                }
                context.stroke();
            }
        }));
    }

    render() {
        const url = this.props.instancesIds.length > 0 ?
            orthancURL + "instances/" + this.props.currentImageId + "/preview" :
            "https://http.cat/404";
        return <>
            {this.props.instancesIds.length > 0 ? (this.props.currentInstanceId + 1) + "/" + this.props.instancesIds.length : null}
            <canvas id="canvas-preview"
                width={this.state.size.width + "px"}
                height={this.state.size.height + "px"}
                style={{
                    backgroundImage: "url(" + url + ")", backgroundSize: "cover"
                }}
                onWheel={(e) => {
                    e.preventDefault();
                    if (e.deltaY < 0) {
                        console.log("scrolling up");
                        this.props.setCurrentInd(
                            this.props.currentInstanceId + 1 >= this.props.instancesIds.length ?
                                this.props.instancesIds.length - 1 :
                                this.props.currentInstanceId + 1
                        );
                    }
                    if (e.deltaY > 0) {
                        console.log("scrolling down");
                        this.props.setCurrentInd(
                            this.props.currentInstanceId - 1 < 0 ? 0 : this.props.currentInstanceId - 1
                        );
                    }
                }}
            />
        </>;
    }
}

export default connect(
    (state: AppState) => {
        return {
            contours: state.contours.filter(c => state.selectedContourGuids.join().includes(c.guid)),
            currentImageId: state.instancesIds[state.currentInstanceId],
            currentInstanceId: state.currentInstanceId,
            instancesIds: state.instancesIds
        };
    },
    (dispatch: Dispatch<any>) => ({
        setCurrentInd: (ind: number) => {
            dispatch(setCurrentInstanceInd(ind));
        },
    })
)(PreviewSavedContours);
