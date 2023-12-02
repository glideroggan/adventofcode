const std = @import("std");

pub fn main() !void {
    var arena = std.heap.ArenaAllocator.init(std.heap.page_allocator);
    const all = arena.allocator();
    defer arena.deinit();

    // get file data
    const file_data = try getFile("test_input.txt", all);

    // parse file data
    const data = try parse(file_data);
    _ = data;
}

fn parse(data: []const u8) !std.ArrayList(Game) {
    // TODO: parse data
    // read each line and split by newline
    // now they are splitted by games
    // parse out game id, then split rest by sets
    // then go through each set and record the highest values for each color
    var games = std.ArrayList(Game).init(std.heap.page_allocator);
    const lines = try splitLines(data);

    for (lines.items) |line| {
        const game = try parseGame(line);
        std.debug.print("Game: {d} r:{d} g:{d} b:{d}\n", .{ game.id, game.red, game.green, game.blue });
        try games.append(game);
    }

    return games;
}

fn parseGame(line: []const u8) !Game {
    // find first space ' ', and then ':', the number between is the game id
    // then split the rest by ';', each part is a set
    // then split each set by ',', each part is a color and a value

    var parts = std.mem.splitAny(u8, line, ":");
    const game_id = try parseId(parts.first());
    var sets

    return Game{
        .id = game_id,
        .red = 0,
        .green = 0,
        .blue = 0,
    };
}

fn parseId(str: []const u8) !u8 {
    std.debug.print("parseId: {s}\n", .{str});
    var buf: [3]u8 = undefined;
    var buf_index: usize = 0;
    for (str) |c| {
        if (std.ascii.isDigit(c)) {
            buf[buf_index] = c;
            buf_index += 1;
        } else if (c == ':') {
            break;
        }
    }
    // std.debug.print("got: {s}", .{buf});
    const num = try std.fmt.parseInt(u8, buf[0..buf_index], 10);
    return num;
}

fn splitLines(data: []const u8) !std.ArrayList([]const u8) {
    var lines = std.ArrayList([]const u8).init(std.heap.page_allocator);

    var line_start: u32 = 0;
    var line_end: u32 = 0;
    while (line_end < data.len) : (line_end += 1) {
        if (data[line_end] == '\n') {
            const line = data[line_start..line_end];
            const l = try std.mem.Allocator.dupeZ(std.heap.page_allocator, u8, line);
            try lines.append(l);
            line_start = line_end + 1;
        }
    }

    return lines;
}

const Game = struct {
    id: u32,
    red: u32,
    green: u32,
    blue: u32,
};

fn getFile(file_name: []const u8, allocator: std.mem.Allocator) ![]const u8 {
    var file = try std.fs.cwd().openFile(file_name, .{ .mode = .read_only });
    defer file.close();

    const file_size = (try file.stat()).size;
    const file_data = try allocator.alloc(u8, file_size);
    _ = try file.read(file_data);

    return file_data;
}
