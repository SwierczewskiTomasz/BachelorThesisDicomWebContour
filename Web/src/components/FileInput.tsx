import * as React from "react";
import { Button, Input } from "@material-ui/core";
import { ButtonProps } from "@material-ui/core/Button";

export interface FileInputButtonProps extends ButtonProps {
    readonly fileType: string;
    readonly onFilesSelect: (files: FileList) => void;
}

export default class FileInputButton extends React.Component<FileInputButtonProps> {
    private inputRef: HTMLInputElement;

    render() {
        const { fileType, onFilesSelect, ...buttonProps } = this.props;

        return (
            <>
                <Input
                    type="file"
                    style={{ display: "none" }}
                    inputRef={ref => this.inputRef = ref}
                    inputProps={{ accept: fileType }}
                    onChange={e => {
                        const input = e.target as HTMLInputElement;
                        input.files && input.files.length > 0 && onFilesSelect(input.files);
                    }}
                />
                <Button {...buttonProps} onClick={() => this.inputRef.click()} variant="contained" color="primary"/>
            </>
        );
    }
}