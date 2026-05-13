import {spawnSync} from 'node:child_process';
import {
    existsSync,
    mkdirSync,
    readdirSync,
    readFileSync,
    rmSync,
    statSync,
    writeFileSync,
} from 'node:fs';
import path from 'node:path';

const [, , appDirectoryArg, stampFileArg, ...outputFileArgs] = process.argv;

if (!appDirectoryArg || !stampFileArg || outputFileArgs.length === 0) {
    console.error('Usage: node BuildFrontend.mjs <app-directory> <stamp-file> <output-file> [output-file...]');
    process.exit(1);
}

const appDirectory = path.resolve(appDirectoryArg);
const stampFile = path.resolve(stampFileArg);
const outputFiles = outputFileArgs.map(outputFileArg => path.resolve(outputFileArg));
const lockDirectory = `${stampFile}.lock`;
const nodeModulesDirectory = path.join(appDirectory, 'node_modules');
const packageLockFile = path.join(appDirectory, 'package-lock.json');
const sourceExclusions = new Set(['node_modules', '.git', 'obj', 'bin', 'wwwroot']);

function sleep(milliseconds) {
    Atomics.wait(new Int32Array(new SharedArrayBuffer(4)), 0, 0, milliseconds);
}

function isProcessRunning(processId) {
    if (!Number.isInteger(processId) || processId <= 0) {
        return false;
    }

    try {
        process.kill(processId, 0);
        return true;
    } catch {
        return false;
    }
}

function shouldRemoveExistingLock() {
    const ownerFile = path.join(lockDirectory, 'owner.txt');

    if (existsSync(ownerFile)) {
        const ownerProcessId = Number.parseInt(readFileSync(ownerFile, 'utf8'), 10);
        if (!isProcessRunning(ownerProcessId)) {
            return true;
        }
    }

    try {
        const lockAgeMilliseconds = Date.now() - statSync(lockDirectory).mtimeMs;
        return lockAgeMilliseconds > 60 * 1000;
    } catch (error) {
        if (error?.code === 'ENOENT') {
            return false;
        }

        throw error;
    }
}

function acquireLock() {
    mkdirSync(path.dirname(stampFile), {recursive: true});
    const ownerFile = path.join(lockDirectory, 'owner.txt');

    const startedAt = Date.now();
    while (true) {
        try {
            mkdirSync(lockDirectory);
        } catch (error) {
            if (error?.code !== 'EEXIST') {
                throw error;
            }

            if (shouldRemoveExistingLock()) {
                rmSync(lockDirectory, {recursive: true, force: true});
                continue;
            }

            if (Date.now() - startedAt > 2 * 60 * 1000) {
                throw new Error(`Timed out waiting for frontend build lock: ${lockDirectory}`);
            }

            sleep(250);
            continue;
        }

        try {
            writeFileSync(ownerFile, `${process.pid}\n`, {encoding: 'utf8', flag: 'wx'});
            return;
        } catch (error) {
            if (error?.code === 'ENOENT' || error?.code === 'EEXIST') {
                continue;
            }

            throw error;
        }
    }
}

function getLatestSourceWriteTime(directory) {
    let latest = statSync(directory).mtimeMs;

    for (const entry of readdirSync(directory, {withFileTypes: true})) {
        if (sourceExclusions.has(entry.name)) {
            continue;
        }

        const entryPath = path.join(directory, entry.name);
        const entryStats = statSync(entryPath);
        latest = Math.max(latest, entryStats.mtimeMs);

        if (entry.isDirectory()) {
            latest = Math.max(latest, getLatestSourceWriteTime(entryPath));
        }
    }

    return latest;
}

function isBuildCurrent() {
    if (!existsSync(stampFile)) {
        return false;
    }

    for (const outputFile of outputFiles) {
        if (!existsSync(outputFile)) {
            return false;
        }
    }

    if (!existsSync(packageLockFile) || !existsSync(nodeModulesDirectory)) {
        return false;
    }

    return statSync(stampFile).mtimeMs >= getLatestSourceWriteTime(appDirectory);
}

function runNpm(args) {
    const npmCommand = process.platform === 'win32' ? 'cmd.exe' : 'npm';
    const npmArgs = process.platform === 'win32'
        ? ['/d', '/s', '/c', 'npm', ...args]
        : args;

    const result = spawnSync(npmCommand, npmArgs, {
        cwd: appDirectory,
        stdio: 'inherit',
    });

    if (result.error) {
        console.error(result.error.message);
        process.exit(1);
    }

    if (result.status !== 0) {
        process.exit(result.status ?? 1);
    }
}

function installDependencies() {
    const hasLockFile = existsSync(packageLockFile);
    runNpm(hasLockFile ? ['ci'] : ['install']);
}

acquireLock();

try {
    if (isBuildCurrent()) {
        console.log('Frontend build output is current.');
        process.exit(0);
    }

    if (!existsSync(nodeModulesDirectory)) {
        installDependencies();
    } else if (process.env.CI === 'true') {
        runNpm(['ci']);
    }

    runNpm(['run', 'build']);
    writeFileSync(stampFile, `${new Date().toISOString()}\n`, 'utf8');
} finally {
    rmSync(lockDirectory, {recursive: true, force: true});
}
