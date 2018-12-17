import * as React from "react";
import { orthancURL } from "../helpers/requestHelper";
import { connect } from "react-redux";
import { Button } from "@material-ui/core";

export interface DisplayImageProps {
    readonly instancesIds: string[];
}

export interface DisplayImageState {
    readonly currentInstanceId: number;
    readonly lastScrollTop: number;
}

class DisplayImage extends React.Component<DisplayImageProps, DisplayImageState> {
    constructor(props: DisplayImageProps) {
        super(props);
        this.state = {
            currentInstanceId: 0,
            lastScrollTop: 0
        };
        console.warn(this.state);
    }

    render() {
        return <>
            {/* <Button
                variant="contained"
                color="primary"
                onClick={() => this.setState(prev => ({ currentInstanceId: prev.currentInstanceId + 1 }))}
            >
                +
            </Button> */}
            <img style={{ margin: "0 auto" }}
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
                        this.setState(prev => ({ currentInstanceId: prev.currentInstanceId + 1 >= this.props.instancesIds.length ? this.props.instancesIds.length - 1 : prev.currentInstanceId + 1 }));
                    }
                    if (e.deltaY > 0) {
                        console.log("scrolling down");
                        this.setState(prev => ({ currentInstanceId: prev.currentInstanceId - 1 < 0 ? 0 : prev.currentInstanceId - 1 }));
                    }
                }}
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
