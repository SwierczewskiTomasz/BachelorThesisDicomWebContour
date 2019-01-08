import * as React from "react";
import { Tabs, Tab, Grid, Typography } from "@material-ui/core";
import DrawManually from "./DrawManually";
import DrawAutomatic from "./DrawAutomatic";
import PatientInfo from "./PatientInfo";

const gridStyle = {
    margin: "2rem"
};

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
                fullWidth
                value={this.state.tab}
                textColor="primary"
                onChange={this.handleChange}
            >
                <Tab label={"manual"} />
                <Tab label={"semi-automatic"} />
            </Tabs>
            <Grid container>
                <Grid item id="pictute" style={{ ...gridStyle }}>
                    {this.state.tab === 0 && <DrawManually />}
                    {this.state.tab === 1 && <DrawAutomatic />}
                </Grid>
                <Grid item id="patient_info" style={{ ...gridStyle }}>
                    <PatientInfo />
                </Grid>
            </Grid>
        </>;
    }
}
