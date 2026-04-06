<template>
	<view class="page">
		<wd-toast />

		<!-- 日期选择 -->
		<view class="date-bar">
			<wd-datetime-picker v-model="dateTs" type="date" label="盘点日期" :maxDate="todayTs"
				:display-format="displayDateFormat" @confirm="onDateConfirm" />
		</view>

		<!-- 加载中 / 空状态 -->
		<view v-if="loadingIngredients" class="center-tip">
			<wd-loading size="28px" />
			<text class="tip-text">加载中...</text>
		</view>

		<view v-else-if="ingredients.length === 0" class="center-tip">
			<wd-status-tip image="search" tip="没有需要盘点的原材料" />
		</view>

		<!-- 原材料列表 -->
		<scroll-view v-else scroll-y class="list-wrap">
			<view class="list-header">请填写各原材料今日剩余数量（不确定的可留空）</view>

			<view class="ingredient-list">
				<view
					v-for="(item, index) in ingredients"
					:key="item.id"
					class="ingredient-row"
					:class="{ 'no-border': index === ingredients.length - 1 }"
				>
					<view class="ingredient-info">
						<text class="ingredient-name">{{ item.name }}</text>
						<text v-if="item.unit" class="ingredient-unit">{{ item.unit }}</text>
					</view>
					<input
						class="qty-input"
						type="digit"
						v-model="quantities[item.id]"
						placeholder="留空"
						placeholder-class="qty-placeholder"
						adjust-position
					/>
				</view>
			</view>

			<view class="list-bottom-pad" />
		</scroll-view>

		<!-- 底部提交 -->
		<view class="submit-bar">
			<wd-button block type="primary" size="large" :loading="submitting"
				:disabled="loadingIngredients || loadingCheck" @click="submit">
				保存盘点记录
			</wd-button>
		</view>
	</view>
</template>

<script setup>
import { ref, reactive, onMounted } from 'vue'
import { useToast } from 'wot-design-uni'
import { api } from '@/utils/api.js'

const toast = useToast()

// ─── 日期 ───────────────────────────────────────────
const todayTs = Date.now()
const dateTs = ref(todayTs)

function tsToDateStr(ts) {
	const d = new Date(ts)
	const y = d.getFullYear()
	const m = String(d.getMonth() + 1).padStart(2, '0')
	const day = String(d.getDate()).padStart(2, '0')
	return `${y}-${m}-${day}`
}

const displayDateFormat = (items) =>
	`${items[0].label}年${items[1].label}月${items[2].label}日`

function onDateConfirm({ value }) {
	dateTs.value = value
	loadCheck(tsToDateStr(value))
}

// ─── 原材料列表 ───────────────────────────────────────
const ingredients = ref([])
const loadingIngredients = ref(false)

async function loadIngredients() {
	loadingIngredients.value = true
	try {
		// consumptionType=1：按库存盘点 的原材料
		const res = await api.get('/api/ingredients', { consumptionType: 1, pageSize: 100 })
		ingredients.value = res.items ?? []
	} catch (e) {
		toast.error(e.message)
	} finally {
		loadingIngredients.value = false
	}
}

// ─── 数量映射 ─────────────────────────────────────────
// { [ingredientId]: number | null }
const quantities = reactive({})

function clearQuantities() {
	ingredients.value.forEach(item => {
		quantities[item.id] = null
	})
}

// ─── 加载已有盘点记录 ────────────────────────────────────
const loadingCheck = ref(false)

async function loadCheck(dateStr) {
	loadingCheck.value = true
	clearQuantities()
	try {
		const res = await api.get(`/api/inventory-checks/${dateStr}`)
		const items = res.items ?? []
		items.forEach(({ ingredientId, quantity }) => {
			if (ingredientId in quantities || ingredients.value.some(i => i.id === ingredientId)) {
				quantities[ingredientId] = quantity
			}
		})
	} catch (e) {
		// 无记录不报错，保持空白
	} finally {
		loadingCheck.value = false
	}
}

// ─── 提交 ─────────────────────────────────────────────
const submitting = ref(false)

async function submit() {
	const dateStr = tsToDateStr(dateTs.value)
	const items = ingredients.value
		.filter(item => quantities[item.id] !== null && quantities[item.id] !== '')
		.map(item => ({ ingredientId: item.id, quantity: Number(quantities[item.id]) }))

	if (items.length === 0) {
		toast.show('请至少填写一项库存数量')
		return
	}

	submitting.value = true
	try {
		await api.put(`/api/inventory-checks/${dateStr}`, { items })
		toast.success('盘点记录已保存')
	} catch (e) {
		toast.error(e.message)
	} finally {
		submitting.value = false
	}
}

// ─── 初始化 ───────────────────────────────────────────
onMounted(async () => {
	await loadIngredients()
	await loadCheck(tsToDateStr(todayTs))
})
</script>

<style lang="scss">
.page {
	display: flex;
	flex-direction: column;
	height: 100vh;
	background: #f5f5f5;
}

.date-bar {
	background: #fff;
	margin-bottom: 12rpx;
}

.center-tip {
	flex: 1;
	display: flex;
	flex-direction: column;
	align-items: center;
	justify-content: center;
	padding-top: 120rpx;
	gap: 20rpx;

	.tip-text {
		font-size: 26rpx;
		color: #999;
	}
}

.list-wrap {
	flex: 1;
	overflow: hidden;
}

.list-bottom-pad {
	height: 140rpx;
}

/* 列表标题 */
.list-header {
	font-size: 24rpx;
	color: #999;
	padding: 24rpx 32rpx 16rpx;
}

/* 原材料行 */
.ingredient-list {
	background: #fff;
	border-radius: 16rpx;
	margin: 0 24rpx;
	overflow: hidden;
}

.ingredient-row {
	display: flex;
	align-items: center;
	justify-content: space-between;
	padding: 28rpx 32rpx;
	border-bottom: 1rpx solid #f0f0f0;

	&.no-border {
		border-bottom: none;
	}
}

.ingredient-info {
	display: flex;
	align-items: baseline;
	gap: 12rpx;
	flex: 1;
}

.ingredient-name {
	font-size: 30rpx;
	color: #333;
}

.ingredient-unit {
	font-size: 22rpx;
	color: #999;
}

.qty-input {
	width: 160rpx;
	text-align: right;
	font-size: 32rpx;
	color: #333;
	border-bottom: 2rpx solid #d0d0d0;
	padding: 4rpx 0;
}

.qty-placeholder {
	color: #ccc;
	font-size: 26rpx;
}

.submit-bar {
	position: fixed;
	bottom: 0;
	left: 0;
	right: 0;
	padding: 20rpx 32rpx;
	padding-bottom: calc(20rpx + env(safe-area-inset-bottom));
	background: #fff;
	box-shadow: 0 -2rpx 12rpx rgba(0, 0, 0, 0.06);
}
</style>
