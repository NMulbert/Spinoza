import React from 'react';
import { Group, Button, Text, Badge, Card, Space } from '@mantine/core';

function ExistingQuestion() {
    return (
        <div>
        <Card withBorder shadow="sm" p="lg">
            <Text weight="bold">Question name</Text>
            <Text size="sm" weight="500">Author</Text>
            <Text size="sm" weight="500">Date</Text>
            <Space h="md"/>
            <Group position="center" spacing="xs">
            <Badge color="pink" variant="light">
            React
            </Badge>
            </Group>
            <Space h="xs"/>
            <Text size="sm" lineClamp={3}>
                Use it to create cards, dropdowns, modals and other components that require background
                with shadow
            </Text>
            <Space h="md"/>
            <Button variant="outline" radius="lg" size="xs">
            Select
            </Button>
        </Card>
        </div>
    );
}

export default ExistingQuestion;