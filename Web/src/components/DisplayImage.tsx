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
    }

    componentWillReceiveProps(props: DisplayImageProps) {
        this.setState(prev => { return ({ reload: !prev.reload }); });
    }
    render() {
        const canvasProps = {
            loadTimeOffset: 5,
            lazyRadius: 0,
            brushRadius: 1,
            brushColor: "#f00",
            catenaryColor: "#0a0302",
            gridColor: "rgba(150,150,150,0.17)",
            hideGrid: true,
            canvasWidth: 400,
            canvasHeight: 400,
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
            {this.state.reload && <CanvasDraw
                {...canvasProps}
                imgSrc={url}
                brushRadius={1}
                style={{ margin: "0 auto" }}
                onWheel={(e) => {
                    if (e.deltaY < 0) {
                        console.log("scrolling up");
                        this.setState(prev => ({ currentInstanceId: prev.currentInstanceId + 1 >= this.props.instancesIds.length ? this.props.instancesIds.length - 1 : prev.currentInstanceId + 1 }));
                    }
                    if (e.deltaY > 0) {
                        console.log("scrolling down");
                        this.setState(prev => ({ currentInstanceId: prev.currentInstanceId - 1 < 0 ? 0 : prev.currentInstanceId - 1 }));
                    }
                }}
            />}
            {!this.state.reload && <div>
                <CanvasDraw
                    {...canvasProps}
                    imgSrc={url}
                    style={{ margin: "0 auto" }}
                    onWheel={(e) => {
                        if (e.deltaY < 0) {
                            console.log("scrolling up");
                            this.setState(prev => ({ currentInstanceId: prev.currentInstanceId + 1 >= this.props.instancesIds.length ? this.props.instancesIds.length - 1 : prev.currentInstanceId + 1 }));
                        }
                        if (e.deltaY > 0) {
                            console.log("scrolling down");
                            this.setState(prev => ({ currentInstanceId: prev.currentInstanceId - 1 < 0 ? 0 : prev.currentInstanceId - 1 }));
                        }
                    }}
                />
            </div>}
            <img style={{ margin: "0 auto" }}
                src={
                    this.props.instancesIds.length > 0 ?
                        orthancURL + "instances/" +
                        this.props.instancesIds[this.state.currentInstanceId]
                        + "/preview" :
                        "https://http.cat/404"
                }
            />
            {/* <Button
                variant="contained"
                color="primary"
                onClick={() => this.setState(prev => ({ currentInstanceId: prev.currentInstanceId - 1 }))}
            >
                -
            </Button> */}
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
