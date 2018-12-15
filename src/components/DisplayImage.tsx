import * as React from "react";
import { orthancURL } from "../helpers/requestHelper";
import { connect } from "react-redux";
import { Button } from "@material-ui/core";

export interface DisplayImageProps {
    readonly instancesIds: string[];
}

export interface DisplayImageState {
    readonly currentInstanceId: number;
}

class DisplayImage extends React.Component<DisplayImageProps, DisplayImageState> {
    constructor(props: DisplayImageProps) {
        super(props);
        this.state = {
            currentInstanceId: 0
        };
        console.warn(this.state);
    }
    render() {
        return <>
            <Button
                variant="contained"
                color="primary"
                onClick={() => this.setState(prev => ({ currentInstanceId: prev.currentInstanceId + 1 }))}
            >
                +
            </Button><img src={this.props.instancesIds.length > 0 ?
                orthancURL + "instances/" +
                this.props.instancesIds[this.state.currentInstanceId % this.props.instancesIds.length]
                + "/preview" :
                "https://http.cat/404"
            }
            />
            <Button
                variant="contained"
                color="primary"
                onClick={() => this.setState(prev => ({ currentInstanceId: prev.currentInstanceId - 1 }))}
            >
                -
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
