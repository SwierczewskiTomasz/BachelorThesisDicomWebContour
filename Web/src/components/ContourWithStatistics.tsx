import * as React from "react";
import { Button, Dialog, DialogContent, DialogActions, DialogTitle, Grid } from "@material-ui/core";
import { Contour } from "../containers/contours/reducers";
import { Size } from "./DrawAutomatic";

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
        if (canvas === null || canvas === undefined || props.contour === undefined || props.contour === null) return;
        console.log(canvas);
        const context = canvas.getContext("2d");

        props.contour.lines.forEach(l => {
            context.fillStyle = l.brushColor;
            for (let i = 0; i < l.points.length; i++) {
                context.fillRect(
                    l.points[0].x * props.size.width / props.contour.width,
                    l.points[0].y * props.size.height / props.contour.height,
                    5, 5);
            }
            for (let i = 0; i < l.pixels.length; i++) {
                const p = l.pixels[i];
                context.fillRect(p.x * props.size.width / props.contour.width,
                    p.y * props.size.height / props.contour.height,
                    2, 2);
            }
            context.stroke();
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
                        <Grid item key="canvas-preview-grid">
                            <canvas id="canvas-preview"
                                width={this.props.size.width + "px"}
                                height={this.props.size.height + "px"}
                                style={{ backgroundImage: this.props.imgUrl, backgroundSize: "cover" }}
                            />
                        </Grid>
                        <Grid item key="stats-grid">
                            {console.log(this.props.contour)}
                            <p><span style={{ ...strong }}>Area</span>: {this.props.contour.statistics.area}</p>
                            <p><span style={{ ...strong }}>CenterOfMass</span>: {"[" + this.props.contour.statistics.centerOfMass.x +
                                "," + this.props.contour.statistics.centerOfMass.x + "]"}</p>
                            <p><span style={{ ...strong }}>HistogramMax</span>: {this.props.contour.statistics.histogramMax}</p>
                            <p><span style={{ ...strong }}>HistogramMin</span>: {this.props.contour.statistics.histogramMin}</p>
                            <p><span style={{ ...strong }}>HistogramMean</span>: {this.props.contour.statistics.histogramMean}</p>
                            <p><span style={{ ...strong }}>Number Of Pixels Inside Contour</span>: {this.props.contour.statistics.numberOfPixelsInsideContour}</p>
                            <p><span style={{ ...strong }}>Permieter</span>: {this.props.contour.statistics.permieter}</p>
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
