import React from "react";
import { Button, ButtonProps, Group } from "@mantine/core";

import { GoogleIcon } from "./GoogleIcon";

import { FacebookIcon } from "./FacebookIcon";

export function GoogleButton(props: ButtonProps<"button">) {
  return (
    <Button
      leftIcon={<GoogleIcon />}
      variant="default"
      color="gray"
      {...props}
    />
  );
}

export function FacebookButton(props: ButtonProps<"button">) {
  return (
    <Button
      leftIcon={<FacebookIcon />}
      sx={(theme) => ({
        backgroundColor: "#4267B2",
        color: "#fff",
        "&:hover": {
          backgroundColor: theme.fn.darken("#4267B2", 0.1),
        },
      })}
      {...props}
    />
  );
}

export function SocialButtons() {
  return (
    <Group position="center" sx={{ padding: 15 }}>
      <GoogleButton>Continue with Google</GoogleButton>
      <FacebookButton>Sign in with Facebook</FacebookButton>
    </Group>
  );
}
