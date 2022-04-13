import React from "react";
import {
  TextInput,
  PasswordInput,
  Checkbox,
  Anchor,
  Paper,
  Title,
  Text,
  Container,
  Group,
  Button,
} from "@mantine/core";
import { FacebookButton, GoogleButton } from "../SocialButtons/SocialButtons";
import { Link } from "react-router-dom";

export function AuthenticationTitle() {
  return (
    <Container size={420} my={40}>
      <Title
        align="center"
        sx={(theme) => ({
          fontFamily: `Greycliff CF, ${theme.fontFamily}`,
          fontWeight: 900,
        })}
      >
        Welcome back!
        <br />
        Login:
      </Title>

      {/* <Text color="dimmed" size="sm" align="center" mt={5}>
        Do not have an account yet?{" "}
        <Anchor<"a">
          href="#"
          size="sm"
          onClick={(event) => event.preventDefault()}
        >
          Create account
        </Anchor>
      </Text> */}

      <Paper withBorder shadow="md" p={30} mt={30} radius="md">
        <Group grow mb="md" mt="md">
          <GoogleButton radius="xl">Google</GoogleButton>
          <FacebookButton radius="xl">Facebook</FacebookButton>
        </Group>
        <tr />
        <TextInput label="Email" placeholder="you@mantine.dev" required />
        <PasswordInput
          label="Password"
          placeholder="Your password"
          required
          mt="md"
        />
        <Group position="apart" mt="md">
          <Checkbox label="Remember me" />

          <Anchor<"a"> onClick={(event) => event.preventDefault()} size="sm">
            <Link to={"/forgot-password"}>Forgot password?</Link>
          </Anchor>
        </Group>
        <Button fullWidth mt="xl">
          Login
        </Button>
      </Paper>

      <Link style={{ textDecoration: "none" }} to={"/dashboard"}>
        <Button fullWidth mt="xl">
          To The Dashboard
        </Button>
      </Link>
    </Container>
  );
}
