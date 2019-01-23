import * as React from "react";
import { Dialog, CircularProgress } from "@material-ui/core";
import { connect } from "react-redux";

interface ProgressSpinnerProps {
    readonly tasksCount: number;
}

class ProgressSpinner extends React.Component<ProgressSpinnerProps> {
    render() {
        return <Dialog
            open={this.props.tasksCount > 0}
            PaperProps={{
                style: {
                    display: "flex",
                    alignItems: "center",
                    justifyContent: "center",
                    width: "5rem",
                    height: "5rem",
                    borderRadius: "2.5rem"
                }
            }}
        >
            <CircularProgress variant={"indeterminate"} />
        </Dialog>;
    }
}

export default connect(
    (state: AppState) => {
        return {
            tasksCount: state.tasksCount
        };
    }
)(ProgressSpinner);
