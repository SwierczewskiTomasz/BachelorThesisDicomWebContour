import * as React from "react";
import { Tabs, Tab } from "@material-ui/core";
import DrawManually from "./DrawManually";
import DrawAutomatic from "./DrawAutomatic";

interface DisplayImageProps {

}
interface DisplayImageState {
    readonly tab: number;
}

export default class DisplayImage extends React.Component<DisplayImageProps, DisplayImageState> {
    constructor(props) {
        super(props);
        this.state = {
            tab: 0
        };
    }
    handleChange = (event, tab) => {
        this.setState({ tab });
        console.warn("tab");
        console.warn(tab);
    }
    render() {
        return <>
            <Tabs
                value={this.state.tab}
                textColor="primary"
                onChange={this.handleChange}
            >
                <Tab label={"manual"} />
                <Tab label={"semi-automatic"} />
            </Tabs>
            {this.state.tab === 0 && <DrawManually />}
            {this.state.tab === 1 && <DrawAutomatic />}
        </>;
    }
}
