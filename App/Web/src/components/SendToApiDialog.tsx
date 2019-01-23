import * as React from "react";
import { Button, Dialog, DialogContent, DialogActions, TextField, DialogTitle } from "@material-ui/core";
import { CompactPicker } from "react-color";
import { Contour } from "../containers/contours/reducers";
import { Point, Size } from "./DrawAutomatic";

interface SendToApiDialogProps {
    readonly open: boolean;
    readonly contour: Contour;
    readonly size: Size;
    readonly imgUrl: string;
    readonly onConfirm: (contour: Contour, centralPoints: Point[], title: string) => void;
    readonly onClose: () => void;
}

interface SendToApiDialogState {
    readonly points: Point[];
    readonly title: string;
    readonly preview: boolean;
}


export default class SendToApiDialog extends React.Component<SendToApiDialogProps, SendToApiDialogState> {
    constructor(props: SendToApiDialogProps) {
        super(props);
        this.state = {
            points: [],
            title: "Contour",
            preview: true
        };
    }

    render() {
        return <Dialog
            maxWidth={"lg"}
            open={this.props.open}
            onClose={() => {
                this.props.onClose();
                this.setState({ preview: true, points: [] });
            }}
        >
            <DialogTitle>
                Insert points inside contour
            </DialogTitle>
            <DialogContent>
                <canvas id="canvas-send"
                    width={this.props.size.width + "px"}
                    height={this.props.size.height + "px"}
                    style={{ backgroundImage: this.props.imgUrl, backgroundSize: "cover" }}
                />
                <TextField
                    margin="dense"
                    label={"Title"}
                    type="text"
                    value={this.state.title}
                    onChange={e => this.setState({ title: e.target.value })}
                    fullWidth
                    autoFocus
                />
            </DialogContent>
            <DialogActions>
                <Button
                    disabled={!this.state.preview}
                    variant={"contained"}
                    color={"primary"}
                    onClick={() => {
                        this.setState({ preview: false });
                        const canvas: any = document.getElementById("canvas-send");

                        console.log(canvas);
                        const context = canvas.getContext("2d");

                        const l = this.props.contour.lines[0];
                        console.warn(l.brushColor);
                        context.strokeStyle = l.brushColor;
                        context.beginPath();
                        context.moveTo(l.points[0].x, l.points[0].y);
                        for (let i = 1; i < l.points.length; i++) {
                            const p = l.points[i];
                            context.lineTo(p.x, p.y);
                        }
                        context.lineTo(l.points[0].x, l.points[0].y);
                        context.stroke();


                        // const l = this.props.contour.lines[0];
                        // console.warn(l.brushColor);
                        // context.fillStyle = l.brushColor;
                        // for (let i = 0; i < l.points.length; i++) {
                        //     const p = l.points[i];
                        //     context.fillRect(p.x, p.y, 2, 2);
                        // }


                        const addPoint = (x, y) => this.setState(prev => { return { points: [...prev.points, { x, y }] }; });
                        const findOverlapping = (x, y) => {
                            const found = this.state.points.filter(p => p.x - 5 < x && x < p.x + 5 && p.y - 5 < y && y < p.y + 5);
                            return found;
                        };
                        const removeOverlapping = (overlapping: Point[]) => {
                            const found = this.state.points.filter(p => !overlapping.find(pp => pp === p));
                            this.setState({ points: found });

                            overlapping.forEach(p => context.clearRect(p.x, p.y, 5, 5));
                        };

                        canvas.addEventListener("click", function (e) {
                            const x = Math.floor(e.offsetX);
                            const y = Math.floor(e.offsetY);

                            const overlapping = findOverlapping(x, y);

                            if (overlapping.length > 0) {
                                removeOverlapping(overlapping);
                            }
                            else {
                                addPoint(x, y);

                                context.fillStyle = "#0000ff";
                                context.fillRect(x, y, 5, 5);
                            }
                        }, true);
                    }}
                >
                    Preview Points
                </Button>
                <Button
                    variant={"contained"}
                    color={"primary"}
                    disabled={this.state.points.length === 0}
                    onClick={() => {
                        this.props.onConfirm(this.props.contour, this.state.points, this.state.title);
                        this.props.onClose();
                        this.setState({ preview: true, points: [] });
                    }}
                >
                    Send
                </Button>
                <Button
                    variant={"contained"}
                    color={"secondary"}
                    onClick={() => {
                        this.props.onClose();
                        this.setState({ preview: true, points: [] });
                    }}
                >
                    Cancel
                </Button>
            </DialogActions>
        </Dialog>;
    }
}
