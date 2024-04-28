import * as fs from 'fs';
import * as path from 'path';

const configPath = path.join(__dirname, '../src/assets/config/config.production.json');

interface AppConfig {
    apiBaseUrl: string;
}

const config: AppConfig = {
    apiBaseUrl: process.env['API_BASE_URL'] || '',
};

fs.writeFile(configPath, JSON.stringify(config, null, 2), err => {
    if (err) {
        console.error('Failed to write config file:', err);
        process.exit(1);
    }

    console.log('Configuration file has been created successfully.');
});
