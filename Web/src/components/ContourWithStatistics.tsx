import * as React from "react";
import { Button, Dialog, DialogContent, DialogActions, DialogTitle, Grid } from "@material-ui/core";
import { Contour } from "../containers/contours/reducers";
import { Size } from "./DrawAutomatic";

const gridStyle = {
    margin: "2rem"
};

interface ContourWithStatisticsProps {
    readonly open: boolean;
    readonly contour: Contour;
    readonly size: Size;
    readonly imgUrl: string;
    readonly onClose: () => void;
}

const strong = {
    fontWeight: 700
};

export default class ContourWithStatistics extends React.Component<ContourWithStatisticsProps> {
    componentDidMount() {
        this.redraw(this.props);
    }

    componentWillReceiveProps(props: ContourWithStatisticsProps) {
        this.redraw(props);
    }

    redraw(props: ContourWithStatisticsProps) {
        const canvas: any = document.getElementById("canvas-preview");
        const canvas2: any = document.getElementById("canvas-histogram");
        if (canvas2 !== null && canvas2 !== undefined && props.contour !== undefined && props.contour !== null) {
            const ctx = canvas2.getContext("2d");
            const max = Math.max(...this.props.contour.statistics.histogram);

            props.contour.statistics.histogram.map(v => v * 255 / max).forEach((v, i) => {
                ctx.strokeStyle = "#fff";
                ctx.beginPath();
                ctx.moveTo(i, 255);
                ctx.lineTo(i, 255 - v);
                ctx.stroke();
            });
        }

        if (canvas === null || canvas === undefined || props.contour === undefined || props.contour === null) return;
        // console.log(canvas);
        const context = canvas.getContext("2d");

        props.contour.lines.forEach(l => {
            if (l.pixels !== undefined) {
                context.fillStyle = l.brushColor;
                for (let i = 0; i < l.points.length; i++) {
                    context.fillRect(
                        l.points[i].x * props.size.width / props.contour.width,
                        l.points[i].y * props.size.height / props.contour.height,
                        5, 5);
                }
                for (let i = 0; i < l.pixels.length; i++) {
                    const p = l.pixels[i];
                    context.fillRect(p.x * props.size.width / props.contour.width,
                        p.y * props.size.height / props.contour.height,
                        2, 2);
                }
            }
            else {
                context.strokeStyle = l.brushColor;
                context.beginPath();
                context.moveTo(l.points[0].x * props.size.width / props.contour.width, l.points[0].y * props.size.height / props.contour.height);
                for (let i = 1; i < l.points.length; i++) {
                    context.lineTo(l.points[i].x * props.size.width / props.contour.width, l.points[i].y * props.size.height / props.contour.height);
                }
                context.stroke();
            }
        });
    }

    render() {
        return <Dialog
            maxWidth={"lg"}
            open={this.props.open}
            onClose={() => {
                this.props.onClose();
            }}
        >
            {this.props.contour && <>
                <DialogTitle>
                    {this.props.contour.tag}
                </DialogTitle>
                <DialogContent>
                    <Grid container>
                        <Grid item key="canvas-preview-grid" style={{ ...gridStyle }}>
                            <canvas id="canvas-preview"
                                width={this.props.size.width + "px"}
                                height={this.props.size.height + "px"}
                                style={{ backgroundImage: this.props.imgUrl, backgroundSize: "cover" }}
                            />
                        </Grid>
                        <Grid item key="stats-grid" style={{ ...gridStyle }}>
                            {/* {console.log(this.props.contour)} */}
                            <p><span style={{ ...strong }}>Permieter</span>: {this.props.contour.statistics.permieter}</p>
                            <p><span style={{ ...strong }}>Area</span>: {this.props.contour.statistics.area}</p>
                            <p><span style={{ ...strong }}>Center Of Mass</span>: {"[" + this.props.contour.statistics.centerOfMass.x +
                                "," + this.props.contour.statistics.centerOfMass.x + "]"}</p>
                            <h1> Histogram </h1>
                            <p><span style={{ ...strong }}>Histogram Max</span>: {this.props.contour.statistics.histogramMax}</p>
                            <p><span style={{ ...strong }}>Histogram Min</span>: {this.props.contour.statistics.histogramMin}</p>
                            <p><span style={{ ...strong }}>Histogram Mean</span>: {this.props.contour.statistics.histogramMean}</p>
                            <p><span style={{ ...strong }}>Number Of Pixels Inside Contour</span>: {this.props.contour.statistics.numberOfPixelsInsideContour}</p>
                            <canvas id="canvas-histogram"
                                width={"256px"}
                                height={"256px"}
                                style={{ background: "#000" }}
                            />
                        </Grid>
                    </Grid>

                </DialogContent>
                <DialogActions>
                    <Button
                        variant={"contained"}
                        color={"secondary"}
                        onClick={() => {
                            this.props.onClose();
                        }}
                    >
                        Close
                </Button>
                </DialogActions>
            </>}
        </Dialog>;
    }
}
