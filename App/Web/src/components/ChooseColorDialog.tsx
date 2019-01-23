import * as React from "react";
import { Button, Dialog, DialogContent, DialogActions } from "@material-ui/core";
import { CompactPicker } from "react-color";

interface ChooseColorDialogProps {
    readonly open: boolean;
    readonly initialColor: string;
    readonly onConfirm: (color: string) => void;
    readonly onClose: () => void;
}
interface ChooseColorDialogState {
    readonly color: string;
}


export default class ChooseColorDialog extends React.Component<ChooseColorDialogProps, ChooseColorDialogState> {
    constructor(props: ChooseColorDialogProps) {
        super(props);
        this.state = {
            color: props.initialColor
        };
    }

    render() {
        return <Dialog
            open={this.props.open}
            onClose={() => this.props.onClose()}
        >
            <DialogContent>
                <CompactPicker
                    // disableAlpha
                    color={this.state.color}
                    onChangeComplete={(c) => { this.setState({ color: c.hex }); }}
                />
            </DialogContent>
            <DialogActions>
                <Button
                    variant={"contained"}
                    color={"primary"}
                    onClick={() => {
                        this.props.onConfirm(this.state.color);
                        this.props.onClose();
                    }}
                >
                    Pick
                </Button>
                <Button
                    variant={"contained"}
                    color={"secondary"}
                    onClick={() => this.props.onClose()}
                >
                    Cancel
                </Button>
            </DialogActions>
        </Dialog>;
    }
}
